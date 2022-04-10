// <copyright file="HostingExtensions.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer;

using System.Reflection;
using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using JD.IdentityServer.Pages;
using Microsoft.EntityFrameworkCore;
using Serilog;

/// <summary>
/// Extension methods for hosting services.
/// </summary>
internal static class HostingExtensions
{
    /// <summary>
    /// Configures the identity server's pipeline.
    /// </summary>
    /// <param name="app"><inheritdoc cref="WebApplication"/></param>
    /// <returns><inheritdoc cref="WebApplication"/></returns>
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        InitializeDatabase(app);

        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer();

        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();

        return app;
    }

    /// <summary>
    /// Configures the identity server client.
    /// </summary>
    /// <param name="builder"><inheritdoc cref="WebApplicationBuilder"/></param>
    /// <returns><inheritdoc cref="WebApplication"/></returns>
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        string _migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
        const string connectionString = @"Data Source=Duende.IdentityServer.Quickstart.EntityFramework.db";

        builder.Services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
            })
            .AddConfigurationStore(opt =>
                opt.ConfigureDbContext = b => b.UseSqlite(connectionString, sql => sql.MigrationsAssembly(_migrationAssembly)))
            .AddOperationalStore(opt =>
                opt.ConfigureDbContext = b => b.UseSqlite(connectionString, sql => sql.MigrationsAssembly(_migrationAssembly)))
            .AddTestUsers(TestUsers.Users);

        builder.Services.AddAuthentication()
            .AddOpenIdConnect("oidc", opt =>
            {
                opt.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                opt.SignOutScheme = IdentityServerConstants.SignoutScheme;
                opt.SaveTokens = true;
                opt.Authority = "https://demo.duendesoftware.com";
                opt.ClientId = "interactive.confidential";
                opt.ClientSecret = "secret";
                opt.ResponseType = "code";
                opt.TokenValidationParameters = new()
                {
                    NameClaimType = "name",
                    RoleClaimType = "role",
                };
            });

        return builder.Build();
    }

    private static void InitializeDatabase(IApplicationBuilder app)
    {
        using var _serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

        _serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
        ConfigurationDbContext _context = _serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        _context.Database.Migrate();
        if (!_context.Clients.Any())
        {
            foreach (var _client in Config.Clients)
            {
                _context.Clients.Add(_client.ToEntity());
            }

            _context.SaveChanges();
        }

        if (!_context.IdentityResources.Any())
        {
            foreach (var _resource in Config.IdentityResources)
            {
                _context.IdentityResources.Add(_resource.ToEntity());
            }

            _context.SaveChanges();
        }

        if (!_context.ApiScopes.Any())
        {
            foreach (var _api in Config.ApiScopes)
            {
                _context.ApiScopes.Add(_api.ToEntity());
            }

            _context.SaveChanges();
        }
    }
}