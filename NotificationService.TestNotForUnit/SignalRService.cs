using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NotificationService.TestNotForUnit;

using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using NotificationService.Domain;

public class SignalRService
{
    private HubConnection _connection;
    private readonly string _jwtToken;

    public SignalRService(string jwtToken)
    {
        _jwtToken = jwtToken;
        GetUserInfoAboutUseByJwt(jwtToken);
        InitializeConnection();
    }

    private void InitializeConnection()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7114/notificationHub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(_jwtToken);
                options.CloseTimeout = TimeSpan.FromSeconds(10);
            })
            .WithAutomaticReconnect(new[] 
            {
                TimeSpan.Zero,    
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10) 
            })
            .Build();
        
        _connection.Closed += OnConnectionClosed;
        _connection.Reconnected += OnReconnected;
        
        _connection.On<Notification>("ReceiveNotification", notification =>
        {
            Console.WriteLine($"Уведомление: {notification.Content}");
        });
    }
    
    public void GetUserInfoAboutUseByJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        
        var jwtToken = handler.ReadJwtToken(token);
        
        var identity = new ClaimsIdentity(jwtToken.Claims);
        var principal = new ClaimsPrincipal(identity);
            
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier);
        var username = principal.FindFirst(ClaimTypes.Name);
        Console.WriteLine($"UserId = {userId.Value}");
    }

    public async Task StartAsync()
    {
        try
        {
            await _connection.StartAsync();
            Console.WriteLine("SignalR подключён.");
            await Task.Delay(-1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка подключения: {ex.Message}");
            await TryRestartConnectionAsync();
        }
    }

    private async Task TryRestartConnectionAsync()
    {
        while (_connection.State != HubConnectionState.Connected)
        {
            try
            {
                await Task.Delay(5000); 
                await _connection.StartAsync();
                return;
            }
            catch
            {
                Console.WriteLine("Повторная попытка подключения...");
            }
        }
    }

    private async Task OnConnectionClosed(Exception? ex)
    {
        
        Console.WriteLine($"Соединение разорвано: {ex?.Message}");
        await TryRestartConnectionAsync();
    }

    private Task OnReconnected(string? connectionId)
    {
        Console.WriteLine($"Переподключено (ID: {connectionId})");
        return Task.CompletedTask;
    }
    
}