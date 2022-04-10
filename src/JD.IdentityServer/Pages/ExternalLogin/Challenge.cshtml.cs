// <copyright file="Challenge.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.ExternalLogin;

using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The Challenge model.
/// </summary>
[AllowAnonymous]
[SecurityHeaders]
public class Challenge : PageModel
{
    private readonly IIdentityServerInteractionService _interactionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="Challenge"/> class.
    /// </summary>
    /// <param name="interactionService"><inheritdoc cref="IIdentityServerInteractionService"/></param>
    public Challenge(IIdentityServerInteractionService interactionService)
    {
        this._interactionService = interactionService;
    }

    /// <summary>
    ///  The method called on HttpGet request.
    /// </summary>
    /// <param name="scheme">The scheme.</param>
    /// <param name="returnUrl">The return url.</param>
    /// <returns>The view.</returns>
    public IActionResult OnGet(string scheme, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = "~/";
        }

        // validate returnUrl - either it is a valid OIDC URL or back to a local page
        if (this.Url.IsLocalUrl(returnUrl) == false && this._interactionService.IsValidReturnUrl(returnUrl) == false)
        {
            // user might have clicked on a malicious link - should be logged
            throw new("invalid return URL");
        }

        // start challenge and roundtrip the return URL and scheme
        AuthenticationProperties _props = new()
        {
            RedirectUri = this.Url.Page("/externallogin/callback"),

            Items =
            {
                { "returnUrl", returnUrl },
                { "scheme", scheme },
            },
        };

        return this.Challenge(_props, scheme);
    }
}