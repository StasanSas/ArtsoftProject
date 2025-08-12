using NotificationService.Application.Interfaces;
using NotificationService.Application.Services;

namespace NotificationService.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSenderNotification(
            this IServiceCollection services)
        {
            services.AddScoped<INotificationSender, NotificationSender>();
            return services;
        }
    }
}