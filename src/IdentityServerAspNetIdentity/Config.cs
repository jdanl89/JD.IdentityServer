// <copyright file="Config.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace IdentityServerAspNetIdentity;

using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

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
        new("api1", "My API"),
    };

    /// <summary>
    /// Gets the identity server clients.
    /// </summary>
    public static IEnumerable<Client> Clients => new[]
    {
        new Client
        {
            ClientId = "client",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets =
            {
                new("secret".Sha256()),
            },
            AllowedScopes =
            {
                "api1",
            },
        },
        new Client
        {
            ClientId = "web",
            ClientSecrets =
            {
                new("secret".Sha256()),
            },
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris =
            {
                "https://localhost:5002/signin-oidc",
            },
            PostLogoutRedirectUris =
            {
                "https://localhost:5002/signout-callback-oidc",
            },
            AllowOfflineAccess = true,
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "verification",
                "api1",
            },
        },
    };

    /// <summary>
    /// Gets the identity resources.
    /// </summary>
    public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
    {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new()
            {
                Name = "verification",
                UserClaims = new List<string>
                {
                    JwtClaimTypes.Email,
                    JwtClaimTypes.EmailVerified,
                },
            },
    };
}