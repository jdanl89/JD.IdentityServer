// <copyright file="Privacy.cshtml.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.WebClient.Pages;

using Microsoft.AspNetCore.Mvc.RazorPages;

/// <summary>
/// The Privacy model.
/// </summary>
public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrivacyModel"/> class.
    /// </summary>
    /// <param name="logger"><inheritdoc cref="ILogger{PrivacyModel}"/></param>
    public PrivacyModel(ILogger<PrivacyModel> logger)
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