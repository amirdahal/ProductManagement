using System.Diagnostics;

namespace ProductManagement.Middlewares;

public class RequestPerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestPerformanceMiddleware> _logger;

    public RequestPerformanceMiddleware(RequestDelegate next, ILogger<RequestPerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Pass the request to next component in pipeline
        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            "HTTP {Method} {Path} executed in {ElapsedMilliseconds}ms",
            context.Request.Method,
            context.Request.Path,
            stopwatch.ElapsedMilliseconds
            );
    }
}
