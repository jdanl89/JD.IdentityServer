// <copyright file="Program.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directives should be placed correctly

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;

#pragma warning restore SA1200 // Using directives should be placed correctly

WebApplicationBuilder _builder = WebApplication.CreateBuilder(args);

// Add services to the container.
_builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

_builder.Services
    .AddAuthentication(opt =>
    {
        opt.DefaultScheme = "Cookies";
        opt.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", opt =>
    {
        opt.Authority = "https://localhost:5001";
        opt.ClientId = "web";
        opt.ClientSecret = "secret";
        opt.ResponseType = "code";
        opt.Scope.Clear();
        opt.Scope.Add("openid");
        opt.Scope.Add("profile");
        opt.Scope.Add("verification");
        opt.Scope.Add("api1");
        opt.Scope.Add("offline_access");
        opt.ClaimActions.MapJsonKey("email_verified", "email_verified");
        opt.GetClaimsFromUserInfoEndpoint = true;
        opt.SaveTokens = true;
    });

WebApplication _app = _builder.Build();

// Configure the HTTP request pipeline.
if (!_app.Environment.IsDevelopment())
{
    _app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _app.UseHsts();
}

_app.UseHttpsRedirection();
_app.UseStaticFiles();

_app.UseRouting();
_app.UseAuthentication();
_app.UseAuthorization();

_app.MapRazorPages().RequireAuthorization();

_app.Run();