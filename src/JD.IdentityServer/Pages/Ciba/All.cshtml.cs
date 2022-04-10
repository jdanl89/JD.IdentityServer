// <copyright file="All.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Ciba;

using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The All model.
/// </summary>
[SecurityHeaders]
[Authorize]
public class AllModel : PageModel
{
    /// <inheritdoc cref="IBackchannelAuthenticationInteractionService"/>
    private readonly IBackchannelAuthenticationInteractionService _backchannelAuthenticationInteraction;

    /// <summary>
    /// Initializes a new instance of the <see cref="AllModel"/> class.
    /// </summary>
    /// <param name="backchannelAuthenticationInteractionService"><inheritdoc cref="IBackchannelAuthenticationInteractionService"/></param>
    public AllModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService)
    {
        this._backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService;
    }

    /// <summary>
    /// Gets or sets the button value.
    /// </summary>
    [BindProperty]
    [Required]
    public string Button { get; set; }

    /// <summary>
    /// Gets or sets the ID.
    /// </summary>
    [BindProperty]
    [Required]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the list of backchannel user login requests.
    /// </summary>
    public IEnumerable<BackchannelUserLoginRequest> Logins { get; set; }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task OnGet()
    {
        this.Logins = await this._backchannelAuthenticationInteraction.GetPendingLoginRequestsForCurrentUserAsync();
    }
}