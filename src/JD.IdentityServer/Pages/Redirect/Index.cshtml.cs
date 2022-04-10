// <copyright file="Index.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Redirect;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The Index model.
/// </summary>
[AllowAnonymous]
public class IndexModel : PageModel
{
    /// <summary>
    /// Gets or sets the redirect uri.
    /// </summary>
    public string RedirectUri { get; set; }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    /// <param name="redirectUri">The redirect uri.</param>
    /// <returns>The redirect view.</returns>
    public IActionResult OnGet(string redirectUri)
    {
        if (!this.Url.IsLocalUrl(redirectUri))
        {
            return this.RedirectToPage("/Error/Index");
        }

        this.RedirectUri = redirectUri;
        return this.Page();
    }
}