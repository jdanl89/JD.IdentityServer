// <copyright file="ViewModel.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Account.Login;

/// <summary>
/// The login view model.
/// </summary>
public class ViewModel
{
    /// <summary>
    /// Gets or sets a value indicating whether to allow the login to be remembered.
    /// </summary>
    public bool AllowRememberLogin { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to enable local login.
    /// </summary>
    public bool EnableLocalLogin { get; set; } = true;

    /// <summary>
    /// Gets the external login scheme.
    /// </summary>
    public string ExternalLoginScheme => this.IsExternalLoginOnly ? this.ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

    /// <summary>
    /// Gets or sets the list of external providers.
    /// </summary>
    public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();

    /// <summary>
    /// Gets a value indicating whether the request is solely for external login.
    /// </summary>
    public bool IsExternalLoginOnly => this.EnableLocalLogin == false && this.ExternalProviders?.Count() == 1;

    /// <summary>
    /// Gets the list of visible external providers.
    /// </summary>
    public IEnumerable<ExternalProvider> VisibleExternalProviders => this.ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

    /// <summary>
    /// The external provider model.
    /// </summary>
    public class ExternalProvider
    {
        /// <summary>
        /// Gets or sets the authentication scheme.
        /// </summary>
        public string AuthenticationScheme { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }
    }
}