// <copyright file="ViewModel.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Ciba;

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
    /// Gets or sets a value indicating whether the consent should be emphasized.
    /// </summary>
    public bool Emphasize { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether consent is required.
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
/// The Ciba view model.
/// </summary>
public class ViewModel
{
    /// <summary>
    /// Gets or sets the list of API scopes.
    /// </summary>
    public IEnumerable<ScopeViewModel> ApiScopes { get; set; }

    /// <summary>
    /// Gets or sets the binding message.
    /// </summary>
    public string BindingMessage { get; set; }

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
    /// Gets or sets the list of identity scopes.
    /// </summary>
    public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
}

/// <summary>
/// Gets or sets the resource view model.
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