// <copyright file="Index.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Ciba;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The index model.
/// </summary>
[AllowAnonymous]
[SecurityHeaders]
public class IndexModel : PageModel
{
    /// <inheritdoc cref="IBackchannelAuthenticationInteractionService"/>>
    private readonly IBackchannelAuthenticationInteractionService _backchannelAuthenticationInteraction;

    /// <inheritdoc cref="ILogger{IndexModel}"/>
    private readonly ILogger<IndexModel> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexModel"/> class.
    /// </summary>
    /// <param name="backchannelAuthenticationInteractionService"><inheritdoc cref="IBackchannelAuthenticationInteractionService"/></param>
    /// <param name="logger"><inheritdoc cref="ILogger{IndexModel}"/></param>
    public IndexModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService, ILogger<IndexModel> logger)
    {
        this._backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService;
        this._logger = logger;
    }

    /// <inheritdoc cref="BackchannelUserLoginRequest"/>
    public BackchannelUserLoginRequest LoginRequest { get; set; }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    /// <param name="id">The ID.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnGet(string id)
    {
        this.LoginRequest = await this._backchannelAuthenticationInteraction.GetLoginRequestByInternalIdAsync(id);
        if (this.LoginRequest == null)
        {
            this._logger.LogWarning("Invalid backchannel login id {id}", id);
            return this.RedirectToPage("/home/error/index");
        }

        return this.Page();
    }
}