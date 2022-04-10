// <copyright file="ViewModel.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.IdentityServer.Pages.Diagnostics;

using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Text.Json;

/// <summary>
/// The viewmodel.
/// </summary>
public class ViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModel"/> class.
    /// </summary>
    /// <param name="result"><inheritdoc cref="AuthenticateResult"/></param>
    public ViewModel(AuthenticateResult result)
    {
        this.AuthenticateResult = result;

        if (result.Properties?.Items.ContainsKey("client_list") ?? false)
        {
            string _encoded = result.Properties.Items["client_list"];
            byte[] _bytes = Base64Url.Decode(_encoded);
            string _value = Encoding.UTF8.GetString(_bytes);

            this.Clients = JsonSerializer.Deserialize<string[]>(_value);
        }
    }

    /// <summary>
    /// Gets the authenticate result.
    /// </summary>
    public AuthenticateResult AuthenticateResult { get; }

    /// <summary>
    /// Gets the identityserver clients.
    /// </summary>
    public IEnumerable<string> Clients { get; } = new List<string>();
}