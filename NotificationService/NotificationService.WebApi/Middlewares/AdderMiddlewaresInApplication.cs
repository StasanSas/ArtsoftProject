namespace NotificationService.WebApi.Middlewares;

public static class AdderMiddlewaresInApplication
{
    public static IApplicationBuilder UseCustomExceptionHandler(this
        IApplicationBuilder builder)
    {
        builder.UseMiddleware<СustomRequestLoggingMiddleware>();
        return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}