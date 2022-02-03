// Copyright (c) EverRise Pte Ltd. All rights reserved.

using EverStats.Services;
using EverStats.Api;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for adding the <see cref="StatsMiddleware"/> to an application.
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds the <see cref="StatsMiddleware"/> 
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    public static IApplicationBuilder UseStats(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        return app.UseMiddleware<StatsMiddleware>();
    }
    /// <summary>
    /// Adds the <see cref="StatsMiddleware"/> 
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    public static IApplicationBuilder UseCoinPricing(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        return app.UseMiddleware<PricingMiddleware>();
    }
}