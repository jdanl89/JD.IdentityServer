// <copyright file="HostingExtensions.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer;

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

        // uncomment if you want to add a UI
        // app.UseStaticFiles();
        // app.UseRouting();
        app.UseIdentityServer();

        // uncomment if you want to add a UI
        // app.UseAuthorization();
        // app.MapRazorPages().RequireAuthorization();
        return app;
    }

    /// <summary>
    /// Configures the identity server client.
    /// </summary>
    /// <param name="builder"><inheritdoc cref="WebApplicationBuilder"/></param>
    /// <returns><inheritdoc cref="WebApplication"/></returns>
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // uncomment if you want to add a UI
        // builder.Services.AddRazorPages();
        builder.Services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients);

        return builder.Build();
    }
}