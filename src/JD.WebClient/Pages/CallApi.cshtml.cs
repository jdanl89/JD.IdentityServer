// <copyright file="CallApi.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.WebClient.Pages
{
    using System.Net.Http.Headers;
    using System.Text.Json;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    /// <summary>
    /// The CallApi model.
    /// </summary>
    public class CallApi : PageModel
    {
        /// <summary>
        /// Gets or sets the JSON string value.
        /// </summary>
        public string Json = string.Empty;

        private static readonly HttpClient Client = new();

        /// <summary>
        /// The method called on HttpGet request.
        /// </summary>
        /// <returns>The view.</returns>
        public async Task OnGet()
        {
            string? _accessToken = await this.HttpContext.GetTokenAsync("access_token");
            Client.DefaultRequestHeaders.Authorization = new("Bearer", _accessToken);
            var _content = await Client.GetStringAsync("https://localhost:6001/identity");
            var _parsed = JsonDocument.Parse(_content);
            var _formatted = JsonSerializer.Serialize(_parsed, new JsonSerializerOptions { WriteIndented = true });
            this.Json = _formatted;
        }
    }
}