using Microsoft.AspNetCore.SignalR;
using NotificationService.Application.Interfaces;
using NotificationService.Domain;
using NotificationService.WebApi.Hubs;

namespace NotificationService.Application.Services;


public class NotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationSender(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task SendNotificationAsync(Notification notification)
    {
        return _hubContext.Clients
            .User(notification.IdRecipient.ToString())
            .SendAsync("ReceiveNotification", notification);
    }
}