// <copyright file="Index.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Account.Logout;

using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using JD.IdentityServer.Pages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The Index model.
/// </summary>
[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly IEventService _events;
    private readonly IIdentityServerInteractionService _interaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class.
    /// </summary>
    /// <param name="interaction"><inheritdoc cref="IIdentityServerInteractionService"/></param>
    /// <param name="events"><inheritdoc cref="IEventService"/></param>
    public Index(IIdentityServerInteractionService interaction, IEventService events)
    {
        this._interaction = interaction;
        this._events = events;
    }

    /// <summary>
    /// Gets or sets the logout ID.
    /// </summary>
    [BindProperty]
    public string LogoutId { get; set; }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    /// <param name="logoutId">The logout ID.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnGet(string logoutId)
    {
        this.LogoutId = logoutId;

        bool _showLogoutPrompt = LogoutOptions.ShowLogoutPrompt;

        if (this.User.Identity?.IsAuthenticated != true)
        {
            // if the user is not authenticated, then just show logged out page
            _showLogoutPrompt = false;
        }
        else
        {
            LogoutRequest _context = await this._interaction.GetLogoutContextAsync(this.LogoutId);
            if (_context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                _showLogoutPrompt = false;
            }
        }

        if (_showLogoutPrompt == false)
        {
            // if the request for logout was properly authenticated from IdentityServer, then
            // we don't need to show the prompt and can just log the user out directly.
            return await this.OnPost();
        }

        return this.Page();
    }

    /// <summary>
    /// The method called on HttpPost request.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnPost()
    {
        if (this.User.Identity?.IsAuthenticated == true)
        {
            // if there's no current logout context, we need to create one
            // this captures necessary info from the current logged in user
            // this can still return null if there is no context needed
            this.LogoutId ??= await this._interaction.CreateLogoutContextAsync();

            // delete local authentication cookie
            await this.HttpContext.SignOutAsync();

            // raise the logout event
            await this._events.RaiseAsync(new UserLogoutSuccessEvent(this.User.GetSubjectId(), this.User.GetDisplayName()));

            // see if we need to trigger federated logout
            string _idp = this.User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            // if it's a local login we can ignore this workflow
            if (_idp != null && _idp != Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider)
            {
                // we need to see if the provider supports external logout
                if (await Extensions.GetSchemeSupportsSignOutAsync(this.HttpContext, _idp))
                {
                    // build a return URL so the upstream provider will redirect back
                    // to us after the user has logged out. this allows us to then
                    // complete our single sign-out processing.
                    string _url = this.Url.Page("/Account/Logout/Loggedout", new { logoutId = this.LogoutId });

                    // this triggers a redirect to the external provider for sign-out
                    return this.SignOut(new AuthenticationProperties { RedirectUri = _url }, _idp);
                }
            }
        }

        return this.RedirectToPage("/Account/Logout/LoggedOut", new { logoutId = this.LogoutId });
    }
}