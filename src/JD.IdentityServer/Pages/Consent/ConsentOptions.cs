// <copyright file="ConsentOptions.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Consent;

/// <summary>
/// The consent options.
/// </summary>
public class ConsentOptions
{
    /// <summary>
    /// Gets the invalid selection error message.
    /// </summary>
    public static readonly string InvalidSelectionErrorMessage = "Invalid selection";

    /// <summary>
    /// Gets the must choose one error message.
    /// </summary>
    public static readonly string MustChooseOneErrorMessage = "You must pick at least one permission";

    /// <summary>
    /// Gets a value indicating whether offline access is enabled.
    /// </summary>
    public static bool EnableOfflineAccess = true;

    /// <summary>
    /// Gets or sets the offline access description.
    /// </summary>
    public static string OfflineAccessDescription = "Access to your applications and resources, even when you are offline";

    /// <summary>
    /// Gets or sets the offline access display name.
    /// </summary>
    public static string OfflineAccessDisplayName = "Offline Access";
}