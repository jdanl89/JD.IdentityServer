// <copyright file="ViewModel.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Grants;

/// <summary>
/// The view model.
/// </summary>
public class ViewModel
{
    /// <summary>
    /// Gets or sets the list of grants.
    /// </summary>
    public IEnumerable<GrantViewModel> Grants { get; set; }
}

/// <summary>
/// The grant view model.
/// </summary>
public class GrantViewModel
{
    /// <summary>
    /// Gets or sets the list of API grant names.
    /// </summary>
    public IEnumerable<string> ApiGrantNames { get; set; }

    /// <summary>
    /// Gets or sets the clientID.
    /// </summary>
    public string ClientId { get; set; }

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
    /// Gets or sets the datetime on which the grant was created.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the datetime on which the grant exprires.
    /// </summary>
    public DateTime? Expires { get; set; }

    /// <summary>
    /// Gets or sets the list of Identity grant names.
    /// </summary>
    public IEnumerable<string> IdentityGrantNames { get; set; }
}