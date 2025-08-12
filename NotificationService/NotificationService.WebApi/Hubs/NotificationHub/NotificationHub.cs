using Microsoft.AspNetCore.SignalR;
using NotificationService.Domain;

namespace NotificationService.WebApi.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotification(Notification notification)
    {
        await Clients.User(notification.IdRecipient.ToString())
            .SendAsync("ReceiveNotification", notification);
    }
}