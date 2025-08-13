using TaskService.WebApi.Middlewares;

namespace TaskService.WebApi.Middleware;

public static class AdderMiddlewaresInApplication
{
    public static IApplicationBuilder UseCustomExceptionHandler(this
        IApplicationBuilder builder)
    {
        
        builder.UseMiddleware<СustomRequestLoggingMiddleware>();
        return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}