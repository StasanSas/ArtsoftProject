namespace TaskService.WebApi.Middleware;

public static class AdderMiddlewaresInApplication
{
    public static IApplicationBuilder UseCustomExceptionHandler(this
        IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}