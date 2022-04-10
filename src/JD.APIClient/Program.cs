// <copyright file="Program.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directives should be placed correctly

using System.Text.Json;
using IdentityModel.Client;

#pragma warning restore SA1200 // Using directives should be placed correctly

//
HttpClient _client = new();

// Get the discovery document that tells us about the identity server.
DiscoveryDocumentResponse _discoveryDoc = await _client.GetDiscoveryDocumentAsync("https://localhost:5001");
if (_discoveryDoc.IsError)
{
    Console.WriteLine(_discoveryDoc.Error);
}

// Get the access token.
TokenResponse _tokenResponse = await _client.RequestClientCredentialsTokenAsync(new()
{
    Address = _discoveryDoc.TokenEndpoint,
    ClientId = "client",
    ClientSecret = "secret",
    Scope = "api1",
});
if (_tokenResponse.IsError)
{
    Console.WriteLine(_tokenResponse.Error);
    Console.WriteLine(_tokenResponse.ErrorDescription);
}

// Set the access token for subsequent HTTP requests.
HttpClient _apiClient = new();
_apiClient.SetBearerToken(_tokenResponse.AccessToken);

// Call the secured API endpoint.
HttpResponseMessage _response = await _apiClient.GetAsync("https://localhost:6001/identity");
if (!_response.IsSuccessStatusCode)
{
    Console.WriteLine(_response.StatusCode);
}
else
{
    JsonElement _doc = JsonDocument.Parse(await _response.Content.ReadAsStringAsync()).RootElement;
    Console.WriteLine(JsonSerializer.Serialize(_doc, new JsonSerializerOptions { WriteIndented = true }));
}

Console.Write("Press any key to close this window . . .");
Console.ReadKey();