// <copyright file="Config.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer;

using Duende.IdentityServer.Models;

/// <summary>
/// The identity server configuration.
/// </summary>
public static class Config
{
    /// <summary>
    /// Gets the API Scopes.
    /// </summary>
    public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
    {
    };

    /// <summary>
    /// Gets the identity server clients.
    /// </summary>
    public static IEnumerable<Client> Clients => new Client[]
    {
    };

    /// <summary>
    /// Gets the identity resources.
    /// </summary>
    public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
    {
            new IdentityResources.OpenId(),
    };
}