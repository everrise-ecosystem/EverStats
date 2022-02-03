// Copyright (c) EverRise Pte Ltd. All rights reserved.

using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using EverStats.Services;

namespace EverStats.Api;
public partial class PricingMiddleware
{
    private ConcurrentDictionary<string, decimal> _prices = new();
    private readonly Stats _stats;
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new <see cref="StatsMiddleware"/>.
    /// </summary>
    /// <param name="next">The <see cref="RequestDelegate"/> representing the next middleware in the pipeline.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> used for logging.</param>
    public PricingMiddleware(Stats stats, RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _stats = stats;
        _logger = loggerFactory.CreateLogger<PricingMiddleware>();
    }

    /// <summary>
    /// Invokes the logic of the middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <returns>A <see cref="Task"/> that completes when the request leaves.</returns>
    public Task Invoke(HttpContext httpContext)
    {
        if (httpContext.Request.Path == "/coin")
        {
            return PriceJson(httpContext);
        }

        return _next(httpContext);
    }

    async Task PriceJson(HttpContext httpContext)
    {
        var query = httpContext.Request.Query;
        var chain = query["chain"].ToString();
        var blockNumber = int.Parse(query["block"]);

        var key = $"{chain}:{blockNumber:0}";
        if (!_prices.TryGetValue(key, out var price))
        {
            price = await _stats.QueryConinPrice(chain, blockNumber);
            if (price != 0)
            {
                _prices[key] = price;
            }
        }

        var json = JsonSerializer.Serialize(new { coinPrice = price });

        var payload = Encoding.UTF8.GetBytes(json);

        var response = httpContext.Response;
        var headers = response.Headers;

        response.StatusCode = 200;
        response.ContentType = "application/json";
        response.ContentLength = payload.Length;

        headers.AccessControlAllowOrigin = "*";
        headers.AccessControlMaxAge = "max-age=31536000";

        await response.BodyWriter.WriteAsync(payload);
    }
}