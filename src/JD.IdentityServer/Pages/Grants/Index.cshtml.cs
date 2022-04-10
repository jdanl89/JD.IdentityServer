// <copyright file="Index.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Grants;

using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The Index model.
/// </summary>
[SecurityHeaders]
[Authorize]
public class Index : PageModel
{
    private readonly IClientStore _clients;
    private readonly IEventService _events;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IResourceStore _resources;

    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class.
    /// </summary>
    /// <param name="interaction"><inheritdoc cref="IIdentityServerInteractionService"/></param>
    /// <param name="clients"><inheritdoc cref="IClientStore"/></param>
    /// <param name="resources"><inheritdoc cref="IResourceStore"/></param>
    /// <param name="events"><inheritdoc cref="IEventService"/></param>
    public Index(
        IIdentityServerInteractionService interaction,
        IClientStore clients,
        IResourceStore resources,
        IEventService events)
    {
        this._interaction = interaction;
        this._clients = clients;
        this._resources = resources;
        this._events = events;
    }

    /// <summary>
    /// Gets or sets the identityserver client ID.
    /// </summary>
    [BindProperty]
    [Required]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the viewmodel.
    /// </summary>
    public ViewModel View { get; set; }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task OnGet()
    {
        IEnumerable<Grant> _grants = await this._interaction.GetAllUserGrantsAsync();

        List<GrantViewModel> _list = new();
        foreach (Grant _grant in _grants)
        {
            Client _client = await this._clients.FindClientByIdAsync(_grant.ClientId);
            if (_client != null)
            {
                Resources _scopeResources = await this._resources.FindResourcesByScopeAsync(_grant.Scopes);

                GrantViewModel _item = new()
                {
                    ClientId = _client.ClientId,
                    ClientName = _client.ClientName ?? _client.ClientId,
                    ClientLogoUrl = _client.LogoUri,
                    ClientUrl = _client.ClientUri,
                    Description = _grant.Description,
                    Created = _grant.CreationTime,
                    Expires = _grant.Expiration,
                    IdentityGrantNames = _scopeResources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                    ApiGrantNames = _scopeResources.ApiScopes.Select(x => x.DisplayName ?? x.Name).ToArray(),
                };

                _list.Add(_item);
            }
        }

        this.View = new()
        {
            Grants = _list,
        };
    }

    /// <summary>
    /// The method called on HttpPost request.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnPost()
    {
        await this._interaction.RevokeUserConsentAsync(this.ClientId);
        await this._events.RaiseAsync(new GrantsRevokedEvent(this.User.GetSubjectId(), this.ClientId));

        return this.RedirectToPage("/Grants/Index");
    }
}