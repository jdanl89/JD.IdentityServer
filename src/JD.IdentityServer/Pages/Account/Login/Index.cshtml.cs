// <copyright file="Index.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Account.Login;

using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The index model.
/// </summary>
[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly IClientStore _clientStore;
    private readonly IEventService _events;
    private readonly IIdentityProviderStore _identityProviderStore;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly TestUserStore _users;

    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class.
    /// </summary>
    /// <param name="interaction"><inheritdoc cref="IIdentityServerInteractionService"/></param>
    /// <param name="clientStore"><inheritdoc cref="IClientStore"/></param>
    /// <param name="schemeProvider"><inheritdoc cref="IAuthenticationSchemeProvider"/></param>
    /// <param name="identityProviderStore"><inheritdoc cref="IIdentityProviderStore"/></param>
    /// <param name="events"><inheritdoc cref="IEventService"/></param>
    /// <param name="users"><inheritdoc cref="TestUserStore"/></param>
    public Index(
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProviderStore identityProviderStore,
        IEventService events,
        TestUserStore users = null)
    {
        // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
        this._users = users ?? throw new("Please call 'AddTestUsers(TestUsers.Users)' on the IIdentityServerBuilder in Startup or remove the TestUserStore from the AccountController.");

        this._interaction = interaction;
        this._clientStore = clientStore;
        this._schemeProvider = schemeProvider;
        this._identityProviderStore = identityProviderStore;
        this._events = events;
    }

    /// <summary>
    /// Gets or sets the input model.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the view model.
    /// </summary>
    public ViewModel View { get; set; }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    /// <param name="returnUrl">The return url.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnGet(string returnUrl)
    {
        await this.BuildModelAsync(returnUrl);

        if (this.View.IsExternalLoginOnly)
        {
            // we only have one option for logging in and it's an external provider
            return this.RedirectToPage("/ExternalLogin/Challenge/Index", new { scheme = this.View.ExternalLoginScheme, returnUrl });
        }

        return this.Page();
    }

    /// <summary>
    /// The method called on HttpPost request.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnPost()
    {
        // check if we are in the context of an authorization request
        AuthorizationRequest _context = await this._interaction.GetAuthorizationContextAsync(this.Input.ReturnUrl);

        // the user clicked the "cancel" button
        if (this.Input.Button != "login")
        {
            if (_context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await this._interaction.DenyAuthorizationAsync(_context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (_context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(this.Input.ReturnUrl);
                }

                return this.Redirect(this.Input.ReturnUrl);
            }
            else
            {
                // since we don't have a valid context, then we just go back to the home page
                return this.Redirect("~/");
            }
        }

        if (this.ModelState.IsValid)
        {
            // validate username/password against in-memory store
            if (this._users.ValidateCredentials(this.Input.Username, this.Input.Password))
            {
                TestUser _user = this._users.FindByUsername(this.Input.Username);
                await this._events.RaiseAsync(new UserLoginSuccessEvent(_user.Username, _user.SubjectId, _user.Username, clientId: _context?.Client.ClientId));

                // only set explicit expiration here if user chooses "remember me".
                // otherwise we rely upon expiration configured in cookie middleware.
                AuthenticationProperties _props = null;
                if (LoginOptions.AllowRememberLogin && this.Input.RememberLogin)
                {
                    _props = new()
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(LoginOptions.RememberMeLoginDuration),
                    };
                }

                // issue authentication cookie with subject ID and username
                IdentityServerUser _isuser = new(_user.SubjectId)
                {
                    DisplayName = _user.Username,
                };

                await this.HttpContext.SignInAsync(_isuser, _props);

                if (_context != null)
                {
                    if (_context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage(this.Input.ReturnUrl);
                    }

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return this.Redirect(this.Input.ReturnUrl);
                }

                // request for a local page
                if (this.Url.IsLocalUrl(this.Input.ReturnUrl))
                {
                    return this.Redirect(this.Input.ReturnUrl);
                }
                else if (string.IsNullOrEmpty(this.Input.ReturnUrl))
                {
                    return this.Redirect("~/");
                }
                else
                {
                    // user might have clicked on a malicious link - should be logged
                    throw new("invalid return URL");
                }
            }

            await this._events.RaiseAsync(new UserLoginFailureEvent(this.Input.Username, "invalid credentials", clientId: _context?.Client.ClientId));
            this.ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
        }

        // something went wrong, show form with error
        await this.BuildModelAsync(this.Input.ReturnUrl);
        return this.Page();
    }

    private async Task BuildModelAsync(string returnUrl)
    {
        this.Input = new()
        {
            ReturnUrl = returnUrl,
        };

        AuthorizationRequest _context = await this._interaction.GetAuthorizationContextAsync(returnUrl);
        if (_context?.IdP != null && await this._schemeProvider.GetSchemeAsync(_context.IdP) != null)
        {
            bool _local = _context.IdP == IdentityServerConstants.LocalIdentityProvider;

            // this is meant to short circuit the UI and only trigger the one external IdP
            this.View = new()
            {
                EnableLocalLogin = _local,
            };

            this.Input.Username = _context.LoginHint;

            if (!_local)
            {
                this.View.ExternalProviders = new[] { new ViewModel.ExternalProvider { AuthenticationScheme = _context.IdP } };
            }
        }

        IEnumerable<AuthenticationScheme> _schemes = await this._schemeProvider.GetAllSchemesAsync();

        List<ViewModel.ExternalProvider> _providers = _schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ViewModel.ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name,
            }).ToList();

        IEnumerable<ViewModel.ExternalProvider> _dynamicSchemes = (await this._identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ViewModel.ExternalProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName,
            });
        _providers.AddRange(_dynamicSchemes);

        bool _allowLocal = true;
        if (_context?.Client.ClientId != null)
        {
            Client _client = await this._clientStore.FindEnabledClientByIdAsync(_context.Client.ClientId);
            if (_client != null)
            {
                _allowLocal = _client.EnableLocalLogin;

                if (_client.IdentityProviderRestrictions != null && _client.IdentityProviderRestrictions.Any())
                {
                    _providers = _providers.Where(provider => _client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                }
            }
        }

        this.View = new()
        {
            AllowRememberLogin = LoginOptions.AllowRememberLogin,
            EnableLocalLogin = _allowLocal && LoginOptions.AllowLocalLogin,
            ExternalProviders = _providers.ToArray(),
        };
    }
}