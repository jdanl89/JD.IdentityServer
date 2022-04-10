// <copyright file="Callback.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.ExternalLogin;

using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The callback model.
/// </summary>
[AllowAnonymous]
[SecurityHeaders]
public class Callback : PageModel
{
    private readonly IEventService _events;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ILogger<Callback> _logger;
    private readonly TestUserStore _users;

    /// <summary>
    /// Initializes a new instance of the <see cref="Callback"/> class.
    /// </summary>
    /// <param name="interaction"><inheritdoc cref="IIdentityServerInteractionService"/></param>
    /// <param name="events"><inheritdoc cref="IEventService"/></param>
    /// <param name="logger"><inheritdoc cref="ILogger"/></param>
    /// <param name="users"><inheritdoc cref="TestUserStore"/></param>
    public Callback(
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<Callback> logger,
        TestUserStore users = null)
    {
        // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
        this._users = users ?? throw new("Please call 'AddTestUsers(TestUsers.Users)' on the IIdentityServerBuilder in Startup or remove the TestUserStore from the AccountController.");

        this._interaction = interaction;
        this._logger = logger;
        this._events = events;
    }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnGet()
    {
        // read external identity from the temporary cookie
        AuthenticateResult _result = await this.HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (_result.Succeeded != true)
        {
            throw new("External authentication error");
        }

        ClaimsPrincipal _externalUser = _result.Principal;

        if (this._logger.IsEnabled(LogLevel.Debug))
        {
            IEnumerable<string> _externalClaims = _externalUser?.Claims.Select(c => $"{c.Type}: {c.Value}");
            this._logger.LogDebug("External claims: {@claims}", _externalClaims);
        }

        // lookup our user and external provider info
        // try to determine the unique id of the external user (issued by the provider)
        // the most common claim type for that are the sub claim and the NameIdentifier
        // depending on the external provider, some other claim type might be used
        Claim _userIdClaim = _externalUser?.FindFirst(JwtClaimTypes.Subject) ??
                              _externalUser?.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new("Unknown userid");

        string _provider = _result.Properties?.Items["scheme"];
        string _providerUserId = _userIdClaim.Value;

        // find external user
        TestUser _user = this._users.FindByExternalProvider(_provider, _providerUserId);
        if (_user == null)
        {
            // this might be where you might initiate a custom workflow for user registration
            // in this sample we don't show how that would be done, as our sample implementation
            // simply auto-provisions new external user
            //
            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            List<Claim> _claims = _externalUser.Claims.ToList();
            _claims.Remove(_userIdClaim);
            _user = this._users.AutoProvisionUser(_provider, _providerUserId, _claims.ToList());
        }

        // this allows us to collect any additional claims or properties
        // for the specific protocols used and store them in the local auth cookie.
        // this is typically used to store data needed for signout from those protocols.
        List<Claim> _additionalLocalClaims = new();
        AuthenticationProperties _localSignInProps = new();
        this.CaptureExternalLoginContext(_result, _additionalLocalClaims, _localSignInProps);

        // issue authentication cookie for user
        IdentityServerUser _isuser = new(_user.SubjectId)
        {
            DisplayName = _user.Username,
            IdentityProvider = _provider,
            AdditionalClaims = _additionalLocalClaims,
        };

        await this.HttpContext.SignInAsync(_isuser, _localSignInProps);

        // delete temporary cookie used during external authentication
        await this.HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // retrieve return URL
        string _returnUrl = _result.Properties?.Items["returnUrl"] ?? "~/";

        // check if external login is in the context of an OIDC request
        AuthorizationRequest _context = await this._interaction.GetAuthorizationContextAsync(_returnUrl);
        await this._events.RaiseAsync(new UserLoginSuccessEvent(_provider, _providerUserId, _user.SubjectId, _user.Username, true, _context?.Client.ClientId));

        if (_context != null)
        {
            if (_context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage(_returnUrl);
            }
        }

        return this.Redirect(_returnUrl);
    }

    // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
    // this will be different for WS-Fed, SAML2p or other protocols
    private void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        Claim _sid = externalResult.Principal?.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (_sid != null)
        {
            localClaims.Add(new(JwtClaimTypes.SessionId, _sid.Value));
        }

        // if the external provider issued an id_token, we'll keep it for signout
        string _idToken = externalResult.Properties?.GetTokenValue("id_token");
        if (_idToken != null)
        {
            localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = _idToken } });
        }
    }
}