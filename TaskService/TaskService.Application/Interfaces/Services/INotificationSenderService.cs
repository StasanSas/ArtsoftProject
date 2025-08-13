using TaskService.Domain;

namespace TaskService.Application.Interfaces.Services;

public interface INotificationSenderService
{
    public void SendNotification(NewNotification newNotification);
}