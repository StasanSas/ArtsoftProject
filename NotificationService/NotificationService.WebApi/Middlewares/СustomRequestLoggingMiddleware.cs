using System.Diagnostics;

namespace NotificationService.WebApi.Middlewares;

public class �ustomRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<�ustomRequestLoggingMiddleware> _logger;

    public �ustomRequestLoggingMiddleware(RequestDelegate next, ILogger<�ustomRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation(
            $"--- Request started ---\n" +
            $"Method: {context.Request.Method} \n" +
            $"Path: {context.Request.Path} \n\n");

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            $"--- Request completed ---\n" +
            $"Method: {context.Request.Method} \n" +
            $"Path: {context.Request.Path} \n" +
            $"Returned status: {context.Response.StatusCode}\n" +
            $"Request processing took {stopwatch.ElapsedMilliseconds} ms");
    }
}