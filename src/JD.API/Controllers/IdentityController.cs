// <copyright file="IdentityController.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// API class requiring bearer auth.
    /// </summary>
    [Route("identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        /// <summary>
        /// Gets the caller's claims.
        /// </summary>
        /// <returns>The caller's claims.</returns>
        public IActionResult Get()
        {
            return new JsonResult(this.User.Claims.Select(c => new { c.Type, c.Value }));
        }
    }
}