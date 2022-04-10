// <copyright file="Index.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Consent;

using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The Index model.
/// </summary>
[Authorize]
[SecurityHeadersAttribute]
public class Index : PageModel
{
    private readonly IEventService _events;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ILogger<Index> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class.
    /// </summary>
    /// <param name="interaction"><inheritdoc cref="IIdentityServerInteractionService"/></param>
    /// <param name="events"><inheritdoc cref="IEventService"/></param>
    /// <param name="logger"><inheritdoc cref="ILogger{Index}"/></param>
    public Index(
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<Index> logger)
    {
        this._interaction = interaction;
        this._events = events;
        this._logger = logger;
    }

    /// <summary>
    /// Gets or sets the input model.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the view model.
    /// </summary>
    public ViewModel View { get; set; }

    /// <summary>
    /// Creates the scope view model.
    /// </summary>
    /// <param name="parsedScopeValue">The parsed scope value.</param>
    /// <param name="apiScope">The API scope.</param>
    /// <param name="check">A value used to set the view model's Checked value.</param>
    /// <returns>The scope view model.</returns>
    public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        string _displayName = apiScope.DisplayName ?? apiScope.Name;
        if (!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
        {
            _displayName += ":" + parsedScopeValue.ParsedParameter;
        }

        return new()
        {
            Name = parsedScopeValue.ParsedName,
            Value = parsedScopeValue.RawValue,
            DisplayName = _displayName,
            Description = apiScope.Description,
            Emphasize = apiScope.Emphasize,
            Required = apiScope.Required,
            Checked = check || apiScope.Required,
        };
    }

    /// <summary>
    ///The method called on HttpPost request.
    /// </summary>
    /// <param name="returnUrl">The return url.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnGet(string returnUrl)
    {
        this.View = await this.BuildViewModelAsync(returnUrl);
        if (this.View == null)
        {
            return this.RedirectToPage("/Error/Index");
        }

        this.Input = new()
        {
            ReturnUrl = returnUrl,
        };

        return this.Page();
    }

    /// <summary>
    /// The method called on HttpPost request.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnPost()
    {
        // validate return url is still valid
        AuthorizationRequest _request = await this._interaction.GetAuthorizationContextAsync(this.Input.ReturnUrl);
        if (_request == null)
        {
            return this.RedirectToPage("/Error/Index");
        }

        ConsentResponse _grantedConsent = null;

        // user clicked 'no' - send back the standard 'access_denied' response
        if (this.Input?.Button == "no")
        {
            _grantedConsent = new() { Error = AuthorizationError.AccessDenied };

            // emit event
            await this._events.RaiseAsync(new ConsentDeniedEvent(this.User.GetSubjectId(), _request.Client.ClientId, _request.ValidatedResources.RawScopeValues));
        }

        // user clicked 'yes' - validate the data
        else if (this.Input?.Button == "yes")
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
            await this._interaction.GrantConsentAsync(_request, _grantedConsent);

            // redirect back to authorization endpoint
            if (_request.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage(this.Input.ReturnUrl);
            }

            return this.Redirect(this.Input.ReturnUrl);
        }

        // we need to redisplay the consent UI
        this.View = await this.BuildViewModelAsync(this.Input.ReturnUrl, this.Input);
        return this.Page();
    }

    private async Task<ViewModel> BuildViewModelAsync(string returnUrl, InputModel model = null)
    {
        AuthorizationRequest _request = await this._interaction.GetAuthorizationContextAsync(returnUrl);
        if (_request != null)
        {
            return this.CreateConsentViewModel(model, returnUrl, _request);
        }
        else
        {
            this._logger.LogError("No consent request matching request: {0}", returnUrl);
        }

        return null;
    }

    private ViewModel CreateConsentViewModel(InputModel model, string returnUrl, AuthorizationRequest request)
    {
        ViewModel _vm = new ViewModel
        {
            ClientName = request.Client.ClientName ?? request.Client.ClientId,
            ClientUrl = request.Client.ClientUri,
            ClientLogoUrl = request.Client.LogoUri,
            AllowRememberConsent = request.Client.AllowRememberConsent,
            IdentityScopes = request.ValidatedResources.Resources.IdentityResources
                .Select(x => this.CreateScopeViewModel(x, model?.ScopesConsented == null || model.ScopesConsented?.Contains(x.Name) == true))
                .ToArray(),
        };

        IEnumerable<string> _resourceIndicators = request.Parameters.GetValues(OidcConstants.AuthorizeRequest.Resource) ?? Enumerable.Empty<string>();
        List<ApiResource> _apiResources = request.ValidatedResources.Resources.ApiResources.Where(x => _resourceIndicators.Contains(x.Name)).ToList();

        List<ScopeViewModel> _apiScopes = new List<ScopeViewModel>();
        foreach (ParsedScopeValue _parsedScope in request.ValidatedResources.ParsedScopes)
        {
            ApiScope _apiScope = request.ValidatedResources.Resources.FindApiScope(_parsedScope.ParsedName);
            if (_apiScope != null)
            {
                ScopeViewModel _scopeVm = this.CreateScopeViewModel(_parsedScope, _apiScope, model == null || model.ScopesConsented?.Contains(_parsedScope.RawValue) == true);
                _scopeVm.Resources = _apiResources.Where(x => x.Scopes.Contains(_parsedScope.ParsedName))
                    .Select(x => new ResourceViewModel
                    {
                        Name = x.Name,
                        DisplayName = x.DisplayName ?? x.Name,
                    }).ToArray();
                _apiScopes.Add(_scopeVm);
            }
        }

        if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
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
            Name = identity.Name,
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
            DisplayName = ConsentOptions.OfflineAccessDisplayName,
            Description = ConsentOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check,
        };
    }
}