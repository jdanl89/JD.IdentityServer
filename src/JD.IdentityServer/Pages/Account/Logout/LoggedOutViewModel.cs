// <copyright file="LoggedOutViewModel.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Account.Logout;

/// <summary>
/// The logged out view model.
/// </summary>
public class LoggedOutViewModel
{
    /// <summary>
    /// Gets or sets a value indicating whether the user should be automatically redirected after sign out.
    /// </summary>
    public bool AutomaticRedirectAfterSignOut { get; set; }

    /// <summary>
    /// Gets or sets the client name.
    /// </summary>
    public string ClientName { get; set; }

    /// <summary>
    /// Gets or sets the post logout redirect uri.
    /// </summary>
    public string PostLogoutRedirectUri { get; set; }

    /// <summary>
    /// Gets or sets the signout iframe url.
    /// </summary>
    public string SignOutIframeUrl { get; set; }
}