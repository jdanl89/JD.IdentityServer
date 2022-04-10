// <copyright file="Signout.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Account.Signout
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    /// <summary>
    /// The Signout model.
    /// </summary>
    public class Signout : PageModel
    {
        /// <summary>
        /// The method called on HttpGet request.
        /// </summary>
        /// <returns>The signout result.</returns>
        public IActionResult OnGet()
        {
            return this.SignOut("Cookies", "oidc");
        }
    }
}