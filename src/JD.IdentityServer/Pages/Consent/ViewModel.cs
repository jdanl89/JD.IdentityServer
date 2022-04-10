// <copyright file="ViewModel.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Consent;

/// <summary>
/// The scope view model.
/// </summary>
public class ScopeViewModel
{
    /// <summary>
    /// Gets or sets a value indicating whether the consent has been checked.
    /// </summary>
    public bool Checked { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the consent is emphasized.
    /// </summary>
    public bool Emphasize { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the consent is required.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the list of resources.
    /// </summary>
    public IEnumerable<ResourceViewModel> Resources { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value { get; set; }
}

/// <summary>
/// The view model.
/// </summary>
public class ViewModel
{
    /// <summary>
    /// Gets or sets a value indicating whether a consent should be allowed to be remembered.
    /// </summary>
    public bool AllowRememberConsent { get; set; }

    /// <summary>
    /// Gets or sets the list of API scopes.
    /// </summary>
    public IEnumerable<ScopeViewModel> ApiScopes { get; set; }

    /// <summary>
    /// Gets or sets the client logo URL.
    /// </summary>
    public string ClientLogoUrl { get; set; }

    /// <summary>
    /// Gets or sets the client name.
    /// </summary>
    public string ClientName { get; set; }

    /// <summary>
    /// Gets or sets the client URL.
    /// </summary>
    public string ClientUrl { get; set; }

    /// <summary>
    /// Gets or sets the list of Identity scopes.
    /// </summary>
    public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
}

/// <summary>
/// The resource view model.
/// </summary>
public class ResourceViewModel
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }
}