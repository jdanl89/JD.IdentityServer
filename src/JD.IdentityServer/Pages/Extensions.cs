// <copyright file="Extensions.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages;

using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// Extension methods.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Determines if the authentication scheme support signout.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="scheme">The scheme.</param>
    /// <returns>A value indicating whether the scheme supports signout.</returns>
    public static async Task<bool> GetSchemeSupportsSignOutAsync(this HttpContext context, string scheme)
    {
        IAuthenticationHandlerProvider _provider = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
        IAuthenticationHandler _handler = await _provider.GetHandlerAsync(context, scheme);
        return _handler is IAuthenticationSignOutHandler;
    }

    /// <summary>
    /// Checks if the redirect URI is for a native client.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <returns>A value indicating whether the client is a native client.</returns>
    public static bool IsNativeClient(this AuthorizationRequest context)
    {
        return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
               && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
    }

    /// <summary>
    /// Renders a loading page that is used to redirect back to the redirectUri.
    /// </summary>
    /// <param name="page">The page model.</param>
    /// <param name="redirectUri">The redirect uri.</param>
    /// <returns>A page redirect to /Redirect/Index with the redirect uri as a query param.</returns>
    public static IActionResult LoadingPage(this PageModel page, string redirectUri)
    {
        page.HttpContext.Response.StatusCode = 200;
        page.HttpContext.Response.Headers["Location"] = string.Empty;

        return page.RedirectToPage("/Redirect/Index", new { RedirectUri = redirectUri });
    }
}