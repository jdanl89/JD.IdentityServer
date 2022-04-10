// <copyright file="LoginOptions.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Account.Login;

/// <summary>
/// The login options.
/// </summary>
public class LoginOptions
{
    /// <summary>
    /// Gets a value indicating whether to allow local login.
    /// </summary>
    public static bool AllowLocalLogin = true;

    /// <summary>
    /// Gets a value indicating whether to allow the login to be remembered.
    /// </summary>
    public static bool AllowRememberLogin = true;

    /// <summary>
    /// Gets the error message to be shown when credentials are invalid.
    /// </summary>
    public static string InvalidCredentialsErrorMessage = "Invalid username or password";

    /// <summary>
    /// Gets the duration for which a user's login should be remembered.
    /// </summary>
    public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
}