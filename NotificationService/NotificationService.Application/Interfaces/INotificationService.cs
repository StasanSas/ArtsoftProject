using NotificationService.Domain;

namespace NotificationService.Application.Interfaces;

public interface INotificationService
{
    public Guid CreateNotification(NewNotification notification);
    
    public IEnumerable<Notification> GetNotifications(Guid idRecipient);
    
    public void PutNotificationAsRead(Guid id);
    
    public Notification GetNotification(Guid id);
}