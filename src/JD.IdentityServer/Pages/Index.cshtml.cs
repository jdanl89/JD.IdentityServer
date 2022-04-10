// <copyright file="Index.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages;

using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The index model.
/// </summary>
[AllowAnonymous]
public class Index : PageModel
{
    /// <summary>
    /// The version.
    /// </summary>
    public string Version;

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    public void OnGet()
    {
        this.Version = typeof(Duende.IdentityServer.Hosting.IdentityServerMiddleware).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+').First();
    }
}