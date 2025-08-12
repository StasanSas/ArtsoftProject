namespace NotificationService.Application;

using Microsoft.Extensions.DependencyInjection;
using Interfaces;
using Services;


public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }
}