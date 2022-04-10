// <copyright file="InputModel.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Ciba;

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
    /// Gets or sets the ID.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the list of consented scopes.
    /// </summary>
    public IEnumerable<string> ScopesConsented { get; set; }
}