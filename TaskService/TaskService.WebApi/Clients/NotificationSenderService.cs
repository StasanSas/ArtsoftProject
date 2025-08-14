using System.Text;
using System.Text.Json;
using TaskService.Application.Interfaces.Services;
using TaskService.Domain;

namespace TaskService.WebApi.Clients;

public class NotificationSenderService : INotificationSenderService
{
    
    private HttpClient _httpClient { get; set; }


    public NotificationSenderService(
        IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("notification");
    }
    
    public async void SendNotification(NewNotification newNotification)
    {
        var jsonContent = JsonSerializer.Serialize(newNotification);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("https://localhost:7091/api/notifications", httpContent);
        
        response.EnsureSuccessStatusCode();
    }
}