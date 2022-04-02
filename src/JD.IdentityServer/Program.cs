// <copyright file="Program.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directives should be placed correctly

using JD.IdentityServer;
using Serilog;

#pragma warning restore SA1200 // Using directives should be placed correctly

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    WebApplicationBuilder _builder = WebApplication.CreateBuilder(args);

    _builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    WebApplication _app = _builder
        .ConfigureServices()
        .ConfigurePipeline();

    _app.Run();
}
catch (Exception _ex)
{
    Log.Fatal(_ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}