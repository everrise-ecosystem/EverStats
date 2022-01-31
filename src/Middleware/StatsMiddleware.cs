// Copyright (c) EverRise Pte Ltd. All rights reserved.

using EverStats.Services;

namespace EverStats.Api;
public partial class StatsMiddleware
{
    private readonly Stats _stats;
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new <see cref="StatsMiddleware"/>.
    /// </summary>
    /// <param name="next">The <see cref="RequestDelegate"/> representing the next middleware in the pipeline.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> used for logging.</param>
    public StatsMiddleware(Stats stats, RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _stats = stats;
        _logger = loggerFactory.CreateLogger<StatsMiddleware>();
    }

    /// <summary>
    /// Invokes the logic of the middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <returns>A <see cref="Task"/> that completes when the request leaves.</returns>
    public Task Invoke(HttpContext httpContext)
    {
        if (httpContext.Request.Path == "/stats")
        {
            return StatsJson(httpContext);
        }

        return _next(httpContext);
    }

    Task StatsJson(HttpContext httpContext)
    {
        var response = httpContext.Response;
        var acceptEncoding = httpContext.Request.Headers.AcceptEncoding.ToString();
        if (acceptEncoding.Length > 64)
        {
            // Not happy parsing, this is far too long
            response.StatusCode = StatusCodes.Status431RequestHeaderFieldsTooLarge;
            return Task.CompletedTask;
        }

        var extraExtension = GetCompressionExtension(acceptEncoding);
        var payload = _stats.JsonBytes;

        var headers = response.Headers;
        switch (extraExtension)
        {
            case ".br":
                payload = _stats.JsonBytesBr;
                headers.ContentEncoding = "br";
                break;
            case ".gz":
                payload = _stats.JsonBytesGzip;
                headers.ContentEncoding = "gzip";
                break;
        };


        response.StatusCode = 200;
        response.ContentType = "application/json";
        response.ContentLength = payload.Length;


        headers.AccessControlAllowOrigin = "*";
        headers.AccessControlMaxAge = "max-age=31536000";

        return response.BodyWriter.WriteAsync(payload).GetAsTask();
    }

    static string GetCompressionExtension(ReadOnlySpan<char> acceptEncoding)
    {
        var extraExtension = string.Empty;
        foreach (var range in acceptEncoding.Split(','))
        {
            var encoding = acceptEncoding[range];
            // Check if is a Quality
            var qualityStart = encoding.IndexOf(';');
            if (qualityStart > 0)
            {
                // Remove Quality
                encoding = encoding[..qualityStart];
            }

            // Remove any additional spaces
            encoding = encoding.Trim(' ');

            if (encoding.SequenceEqual("br"))
            {
                // Brotli accepted, set the additional file extension
                extraExtension = ".br";
                // This is our preferred compression so exit the loop
                break;
            }
            else if (encoding.SequenceEqual("gzip"))
            {
                // Gzip accepted, we'll set the extension, but keep looking
                extraExtension = ".gz";
            }
        }

        return extraExtension;
    }
}