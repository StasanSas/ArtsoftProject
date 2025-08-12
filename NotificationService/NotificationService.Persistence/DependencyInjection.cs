using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces;
using NotificationService.Persistence.DbContexts;

namespace AuthService.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddDb(
        this IServiceCollection services, string connectionString)
    {
        services.AddScoped<INotificationDbContext>(_ => new NotificationDbContext(connectionString));
        return services;
    }
}