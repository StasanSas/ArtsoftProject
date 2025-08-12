using TaskService.Application.Interfaces.DbContexts;
using TaskService.Persistence.DbContexts;

namespace TaskService.Persistence;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddDb(
        this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IJobDbContext>(_ => new JobDbContext(connectionString));
        services.AddScoped<IJobHistoryDbContext>(_ => new JobHistoryDbContext(connectionString));
        
        return services;
    }
}