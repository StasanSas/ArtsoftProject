using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using AuthService.Application.CustomException;
using AuthService.Persistence.CustomException;

namespace AuthService.WebApi.Middlewares;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    //private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public CustomExceptionHandlerMiddleware(
            RequestDelegate next)
        //ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        //_logger = logger;

    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }
    
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode code = HttpStatusCode.InternalServerError;
        var result = string.Empty;
        switch(exception)
        {
            case ValidationException:
                code = HttpStatusCode.BadRequest;
                break;
            case NotCorrectFormatJwtToken:
                code = HttpStatusCode.BadRequest;
                break;
            case NotFoundException:
                code = HttpStatusCode.NotFound;
                break;
        }
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        if (result == string.Empty)
        {
            result = JsonSerializer.Serialize(new { error = exception.Message });
        }

        return context.Response.WriteAsync(result);
    }
    
}