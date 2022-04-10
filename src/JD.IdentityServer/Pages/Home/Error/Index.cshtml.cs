// <copyright file="Index.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Home.Error;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The index model.
/// </summary>
[AllowAnonymous]
[SecurityHeaders]
public class Index : PageModel
{
    private readonly IWebHostEnvironment _environment;
    private readonly IIdentityServerInteractionService _interaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class.
    /// </summary>
    /// <param name="interaction"><inheritdoc cref="IIdentityServerInteractionService"/></param>
    /// <param name="environment"><inheritdoc cref="IWebHostEnvironment"/></param>
    public Index(IIdentityServerInteractionService interaction, IWebHostEnvironment environment)
    {
        this._interaction = interaction;
        this._environment = environment;
    }

    /// <summary>
    /// Gets or sets the viewmodel.
    /// </summary>
    public ViewModel View { get; set; }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    /// <param name="errorId">The error ID.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task OnGet(string errorId)
    {
        this.View = new();

        // retrieve error details from identityserver
        ErrorMessage _message = await this._interaction.GetErrorContextAsync(errorId);
        if (_message != null)
        {
            this.View.Error = _message;

            if (!this._environment.IsDevelopment())
            {
                // only show in development
                _message.ErrorDescription = null;
            }
        }
    }
}