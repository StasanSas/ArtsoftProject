using NotificationService.Application.CustomException;
using NotificationService.Application.Interfaces;
using NotificationService.Domain;

namespace NotificationService.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationDbContext _notificationDbContext;
    private readonly INotificationSender _notificationSender;

    public NotificationService(
        INotificationDbContext notificationDbContext,
        INotificationSender notificationSender)
    {
        _notificationDbContext = notificationDbContext;
        _notificationSender = notificationSender;
    }

    public Guid CreateNotification(NewNotification notification)
    {
        var id = _notificationDbContext.CreateNotification(notification);
        var createdNotification = _notificationDbContext.GetNotification(id);
        if (createdNotification == null)
            throw new InternalDbException("Not found notification after create");
        _notificationSender.SendNotificationAsync(createdNotification);

        return id;
    }

    public IEnumerable<Notification> GetNotifications(Guid idRecipient)
    {
        return _notificationDbContext.GetNotifications(idRecipient);
    }

    public void PutNotificationAsRead(Guid id)
    {
        var notificationNotUpdated = _notificationDbContext.GetNotification(id);
        if (notificationNotUpdated == null)
            throw new NotFoundException("Not found Notification");
        _notificationDbContext.PutNotificationAsRead(id);
        var notificationUpdated = _notificationDbContext.GetNotification(id);
        if (notificationUpdated == null)
            throw new InternalDbException("Not found Notification after update");
        _notificationSender.SendNotificationAsync(notificationUpdated);
    }

    public Notification GetNotification(Guid id)
    {
        var notification = _notificationDbContext.GetNotification(id);
        if (notification == null)
            throw new NotFoundException("Not found Notification");
        return notification;
    }
}