// <copyright file="LoggedOut.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Account.Logout;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The Logged Out model.
/// </summary>
[SecurityHeaders]
[AllowAnonymous]
public class LoggedOut : PageModel
{
    private readonly IIdentityServerInteractionService _interactionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggedOut"/> class.
    /// </summary>
    /// <param name="interactionService"><inheritdoc cref="IIdentityServerInteractionService"/></param>
    public LoggedOut(IIdentityServerInteractionService interactionService)
    {
        this._interactionService = interactionService;
    }

    /// <summary>
    /// Gets or sets the view model.
    /// </summary>
    public LoggedOutViewModel View { get; set; }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    /// <param name="logoutId">The logout ID.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task OnGet(string logoutId)
    {
        // get context information (client name, post logout redirect URI and iframe for federated sign-out)
        LogoutRequest _logout = await this._interactionService.GetLogoutContextAsync(logoutId);

        this.View = new()
        {
            AutomaticRedirectAfterSignOut = LogoutOptions.AutomaticRedirectAfterSignOut,
            PostLogoutRedirectUri = _logout?.PostLogoutRedirectUri,
            ClientName = string.IsNullOrEmpty(_logout?.ClientName) ? _logout?.ClientId : _logout.ClientName,
            SignOutIframeUrl = _logout?.SignOutIFrameUrl,
        };
    }
}