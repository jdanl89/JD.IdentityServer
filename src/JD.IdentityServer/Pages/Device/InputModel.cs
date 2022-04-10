// <copyright file="InputModel.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Device;

/// <summary>
/// The Input model.
/// </summary>
public class InputModel
{
    /// <summary>
    /// Gets or sets the button value.
    /// </summary>
    public string Button { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the consent should be remembered.
    /// </summary>
    public bool RememberConsent { get; set; } = true;

    /// <summary>
    /// Gets or sets the return url.
    /// </summary>
    public string ReturnUrl { get; set; }

    /// <summary>
    /// Gets or sets the list of consented scopes.
    /// </summary>
    public IEnumerable<string> ScopesConsented { get; set; }

    /// <summary>
    /// Gets or sets the user code.
    /// </summary>
    public string UserCode { get; set; }
}