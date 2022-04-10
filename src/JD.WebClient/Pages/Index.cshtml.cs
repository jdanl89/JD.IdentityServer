// <copyright file="Index.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.WebClient.Pages;

using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The Index model.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexModel"/> class.
    /// </summary>
    /// <param name="logger"><inheritdoc cref="ILogger{IndexModel}"/></param>
    public IndexModel(ILogger<IndexModel> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// The method called on HttpGet request.
    /// </summary>
    public void OnGet()
    {
    }
}