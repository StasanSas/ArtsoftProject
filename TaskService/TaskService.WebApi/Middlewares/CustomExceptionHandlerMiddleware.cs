using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using TaskService.Persistence.CustomException;

namespace TaskService.WebApi.Middlewares;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

    public CustomExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception exception)
        {
            await HandleExceptionAsync(context, exception, context);
        }
    }
    
    private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpContext httpContext)
    {
        HttpStatusCode code = HttpStatusCode.InternalServerError;
        
        switch(exception)
        {
            case ValidationException:
                code = HttpStatusCode.BadRequest;
                break;
            case NotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            case ExistAlreadyException:
                code = HttpStatusCode.Conflict;
                break;
        }

        _logger.LogError(
            "Произошло исключение \n" +
            $"Сообщение ошибки {exception.Message} \n" +
            $"Стек ошибки {exception.StackTrace ?? "N\\A"} \n");

        var result = JsonSerializer.Serialize(new { error = exception.Message });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}