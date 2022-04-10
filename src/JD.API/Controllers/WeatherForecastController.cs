// <copyright file="WeatherForecastController.cs" company="JasonDanley.com">
// Copyright (c) JasonDanley.com. All rights reserved.
// </copyright>

namespace JD.API.Controllers;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// The API controller for retrieving weather forecasts.
/// </summary>
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
    };

    private readonly ILogger<WeatherForecastController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherForecastController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// Gets the weather forecast.
    /// </summary>
    /// <returns><inheritdoc cref="WeatherForecast"/></returns>
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        this._logger.Log(LogLevel.Trace, "Getting the weather forecast.");

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
        })
        .ToArray();
    }
}