using TaskService.Application.Interfaces.Services;
using TaskService.WebApi.Clients;
using TaskService.WebApi.HttpServices;

namespace TaskService.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(
        this IServiceCollection services)
    {
        
        services.AddScoped<INotificationSenderService, NotificationSenderService>();
        services.AddScoped<IUserJwtTokenHttpService, UserJwtTokenHttpService>();
        return services;
    }
}