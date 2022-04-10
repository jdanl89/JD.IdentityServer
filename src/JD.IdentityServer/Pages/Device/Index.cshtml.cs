// <copyright file="Index.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Device;

using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using JD.IdentityServer.Pages.Consent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

/// <summary>
/// The Index model.
/// </summary>
[SecurityHeaders]
[Authorize]
public class Index : PageModel
{
    private readonly IEventService _events;
    private readonly IDeviceFlowInteractionService _interaction;
    private readonly ILogger<Index> _logger;
    private readonly IOptions<IdentityServerOptions> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class.
    /// </summary>
    /// <param name="interaction"><inheritdoc cref="IDeviceFlowInteractionService"/></param>
    /// <param name="eventService"><inheritdoc cref="IEventService"/></param>
    /// <param name="options"><inheritdoc cref="IOptions{IdentityServerOptions}"/></param>
    /// <param name="logger"><inheritdoc cref="ILogger{Index}"/></param>
    public Index(
        IDeviceFlowInteractionService interaction,
        IEventService eventService,
        IOptions<IdentityServerOptions> options,
        ILogger<Index> logger)
    {
        this._interaction = interaction;
        this._events = eventService;
        this._options = options;
        this._logger = logger;
    }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the viewmodel.
    /// </summary>
    public ViewModel View { get; set; }

    /// <summary>
    /// Creates the scoped view model.
    /// </summary>
    /// <param name="parsedScopeValue"><inheritdoc cref="ParsedScopeValue"/></param>
    /// <param name="apiScope"><inheritdoc cref="ApiScope"/></param>
    /// <param name="check">The value used to determine the view model's Checked value.</param>
    /// <returns><inheritdoc cref="ScopeViewModel"/></returns>
    public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        return new()
        {
            // todo: use the parsed scope value in the display?
            Value = parsedScopeValue.RawValue,
            DisplayName = apiScope.DisplayName ?? apiScope.Name,
            Description = apiScope.Description,
            Emphasize = apiScope.Emphasize,
            Required = apiScope.Required,
            Checked = check || apiScope.Required,
        };
    }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    /// <param name="userCode">The user code.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnGet(string userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode))
        {
            this.View = new();
            this.Input = new();
            return this.Page();
        }

        this.View = await this.BuildViewModelAsync(userCode);
        if (this.View == null)
        {
            this.ModelState.AddModelError(string.Empty, DeviceOptions.InvalidUserCode);
            this.View = new();
            this.Input = new();
            return this.Page();
        }

        this.Input = new()
        {
            UserCode = userCode,
        };

        return this.Page();
    }

    /// <summary>
    /// The method called on HttpPost request.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnPost()
    {
        DeviceFlowAuthorizationRequest _request = await this._interaction.GetAuthorizationContextAsync(this.Input.UserCode);
        if (_request == null)
        {
            return this.RedirectToPage("/Error/Index");
        }

        ConsentResponse _grantedConsent = null;

        // user clicked 'no' - send back the standard 'access_denied' response
        if (this.Input.Button == "no")
        {
            _grantedConsent = new()
            {
                Error = AuthorizationError.AccessDenied,
            };

            // emit event
            await this._events.RaiseAsync(new ConsentDeniedEvent(this.User.GetSubjectId(), _request.Client.ClientId, _request.ValidatedResources.RawScopeValues));
        }

        // user clicked 'yes' - validate the data
        else if (this.Input.Button == "yes")
        {
            // if the user consented to some scope, build the response model
            if (this.Input.ScopesConsented != null && this.Input.ScopesConsented.Any())
            {
                IEnumerable<string> _scopes = this.Input.ScopesConsented;
                if (ConsentOptions.EnableOfflineAccess == false)
                {
                    _scopes = _scopes.Where(x => x != Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess);
                }

                _grantedConsent = new()
                {
                    RememberConsent = this.Input.RememberConsent,
                    ScopesValuesConsented = _scopes.ToArray(),
                    Description = this.Input.Description,
                };

                // emit event
                await this._events.RaiseAsync(new ConsentGrantedEvent(this.User.GetSubjectId(), _request.Client.ClientId, _request.ValidatedResources.RawScopeValues, _grantedConsent.ScopesValuesConsented, _grantedConsent.RememberConsent));
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, ConsentOptions.MustChooseOneErrorMessage);
            }
        }
        else
        {
            this.ModelState.AddModelError(string.Empty, ConsentOptions.InvalidSelectionErrorMessage);
        }

        if (_grantedConsent != null)
        {
            // communicate outcome of consent back to identityserver
            await this._interaction.HandleRequestAsync(this.Input.UserCode, _grantedConsent);

            // indicate that's it ok to redirect back to authorization endpoint
            return this.RedirectToPage("/Device/Success");
        }

        // we need to redisplay the consent UI
        this.View = await this.BuildViewModelAsync(this.Input.UserCode, this.Input);
        return this.Page();
    }

    private async Task<ViewModel> BuildViewModelAsync(string userCode, InputModel model = null)
    {
        DeviceFlowAuthorizationRequest _request = await this._interaction.GetAuthorizationContextAsync(userCode);
        if (_request != null)
        {
            return this.CreateConsentViewModel(model, _request);
        }

        return null;
    }

    private ViewModel CreateConsentViewModel(InputModel model, DeviceFlowAuthorizationRequest request)
    {
        ViewModel _vm = new()
        {
            ClientName = request.Client.ClientName ?? request.Client.ClientId,
            ClientUrl = request.Client.ClientUri,
            ClientLogoUrl = request.Client.LogoUri,
            AllowRememberConsent = request.Client.AllowRememberConsent,
            IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => this.CreateScopeViewModel(x, model == null || model.ScopesConsented?.Contains(x.Name) == true)).ToArray(),
        };

        List<ScopeViewModel> _apiScopes = new();
        foreach (ParsedScopeValue _parsedScope in request.ValidatedResources.ParsedScopes)
        {
            ApiScope _apiScope = request.ValidatedResources.Resources.FindApiScope(_parsedScope.ParsedName);
            if (_apiScope != null)
            {
                ScopeViewModel _scopeVm = this.CreateScopeViewModel(_parsedScope, _apiScope, model == null || model.ScopesConsented?.Contains(_parsedScope.RawValue) == true);
                _apiScopes.Add(_scopeVm);
            }
        }

        if (DeviceOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
        {
            _apiScopes.Add(this.GetOfflineAccessScope(model == null || model.ScopesConsented?.Contains(Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess) == true));
        }

        _vm.ApiScopes = _apiScopes;

        return _vm;
    }

    private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
    {
        return new()
        {
            Value = identity.Name,
            DisplayName = identity.DisplayName ?? identity.Name,
            Description = identity.Description,
            Emphasize = identity.Emphasize,
            Required = identity.Required,
            Checked = check || identity.Required,
        };
    }

    private ScopeViewModel GetOfflineAccessScope(bool check)
    {
        return new()
        {
            Value = Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess,
            DisplayName = DeviceOptions.OfflineAccessDisplayName,
            Description = DeviceOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check,
        };
    }
}