namespace NotificationService.WebApi.Middlewares;

public static class AdderMiddlewaresInApplication
{
    public static IApplicationBuilder UseCustomExceptionHandler(this
        IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}