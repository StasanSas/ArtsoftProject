using System.Diagnostics;

namespace NotificationService.WebApi.Middlewares;

public class СustomRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<СustomRequestLoggingMiddleware> _logger;

    public СustomRequestLoggingMiddleware(RequestDelegate next, ILogger<СustomRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation(
            $"---Начало запроса---\n" +
            $"Метод: {context.Request.Method} \n" +
            $"Путь: {context.Request.Path} \n \n");

        await _next(context); 

        stopwatch.Stop();
        
        _logger.LogInformation(
            $"---Запрос завершён---\n" +
            $"Метод: {context.Request.Method} \n" +
            $"Путь: {context.Request.Path} \n" +
            $"Возвращённый статус: {context.Response.StatusCode}\n" +
                $"Обработка запроса заняла {stopwatch.ElapsedMilliseconds}");
    }
}