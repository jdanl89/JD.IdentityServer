// <copyright file="Index.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Diagnostics;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// The Index model.
/// </summary>
[SecurityHeaders]
[Authorize]
public class Index : PageModel
{
    /// <summary>
    /// Gets or sets the view model.
    /// </summary>
    public ViewModel View { get; set; }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnGet()
    {
        string[] _localAddresses = new[] { "127.0.0.1", "::1", this.HttpContext.Connection.LocalIpAddress?.ToString() };
        if (!_localAddresses.Contains(this.HttpContext.Connection.RemoteIpAddress?.ToString()))
        {
            return this.NotFound();
        }

        this.View = new(await this.HttpContext.AuthenticateAsync());

        return this.Page();
    }
}