// <copyright file="ViewModel.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Home.Error;

using Duende.IdentityServer.Models;

/// <summary>
/// The view model.
/// </summary>
public class ViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModel"/> class.
    /// </summary>
    public ViewModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModel"/> class.
    /// </summary>
    /// <param name="error">The error message.</param>
    public ViewModel(string error)
    {
        this.Error = new() { Error = error };
    }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public ErrorMessage Error { get; set; }
}