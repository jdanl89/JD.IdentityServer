// <copyright file="Success.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Device;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The success model.
/// </summary>
[SecurityHeaders]
[Authorize]
public class SuccessModel : PageModel
{
    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    public void OnGet()
    {
    }
}