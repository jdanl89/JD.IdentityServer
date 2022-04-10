// <copyright file="LogoutOptions.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Account.Logout;

/// <summary>
/// The logout options.
/// </summary>
public class LogoutOptions
{
    /// <summary>
    /// Gets a value indicating whether the user should be automatically redirected after signout.
    /// </summary>
    public static bool AutomaticRedirectAfterSignOut = false;

    /// <summary>
    /// Gets a value indicating whether the logout prompt should be shown.
    /// </summary>
    public static bool ShowLogoutPrompt = true;
}