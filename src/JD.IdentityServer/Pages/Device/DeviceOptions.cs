// <copyright file="DeviceOptions.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Device;

/// <summary>
/// The device options.
/// </summary>
public class DeviceOptions
{
    /// <summary>
    /// The invalid selection error message.
    /// </summary>
    public static readonly string InvalidSelectionErrorMessage = "Invalid selection";

    /// <summary>
    /// The invalid user code message.
    /// </summary>
    public static readonly string InvalidUserCode = "Invalid user code";

    /// <summary>
    /// The must choose one error message.
    /// </summary>
    public static readonly string MustChooseOneErrorMessage = "You must pick at least one permission";

    /// <summary>
    /// A value indicating whether offline access is enabled.
    /// </summary>
    public static bool EnableOfflineAccess = true;

    /// <summary>
    /// The offline access description.
    /// </summary>
    public static string OfflineAccessDescription = "Access to your applications and resources, even when you are offline";

    /// <summary>
    /// The offline access display name.
    /// </summary>
    public static string OfflineAccessDisplayName = "Offline Access";
}