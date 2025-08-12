using NotificationService.Domain;
using NotificationService.WebApi.Dtos;

namespace NotificationService.WebApi.Mappers;

public static class Mapper
{
    public static NotificationWithIdDto ToNotificationWithIdDto(this Notification notification)
    {
        return new NotificationWithIdDto()
        {
            Id = notification.Id,
            IdSender = notification.IdSender,
            IdRecipient = notification.IdRecipient,
            Content = notification.Content
        };
    }
}