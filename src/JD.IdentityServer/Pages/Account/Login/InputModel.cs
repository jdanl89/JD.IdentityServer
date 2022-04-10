// <copyright file="InputModel.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Account.Login;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// The input model.
/// </summary>
public class InputModel
{
    /// <summary>
    /// Gets or sets the button text.
    /// </summary>
    public string Button { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the login should be remembered.
    /// </summary>
    public bool RememberLogin { get; set; }

    /// <summary>
    /// Gets or sets the return url.
    /// </summary>
    public string ReturnUrl { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    [Required]
    public string Username { get; set; }
}