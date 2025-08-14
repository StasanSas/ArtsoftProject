using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TaskService.Application.Interfaces;
using TaskService.Application.Interfaces.Services;
using TaskService.Application.Services;

namespace TaskService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<JobService>();
        services.AddScoped<IJobService>(sp => sp.GetRequiredService<JobService>());
        services.AddScoped<IJobEventPublisher>(sp => sp.GetRequiredService<JobService>());
        services.AddScoped<IJobHistoryService, JobHistoryService>();
        return services;
    }
}