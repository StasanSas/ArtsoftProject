using NotificationService.Domain;

namespace NotificationService.Application.Interfaces;

public interface INotificationSender
{
    Task SendNotificationAsync(Notification notification);
}