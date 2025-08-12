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
        services.AddScoped<IJobService,JobService>();
        services.AddScoped<IJobHistoryService, JobHistoryService>();
        services.AddScoped<IJobEventPublisher, JobService>();
        return services;
    }
}