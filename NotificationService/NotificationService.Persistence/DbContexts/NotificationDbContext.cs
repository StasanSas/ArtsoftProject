using Dapper;
using NotificationService.Application.Interfaces;
using NotificationService.Domain;
using Npgsql;

namespace NotificationService.Persistence.DbContexts;

public class NotificationDbContext : INotificationDbContext
{
    
    private readonly string _connectionString;

    public NotificationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public Guid CreateNotification(NewNotification notification)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var sql = @"
            INSERT INTO notifications (id_sender, id_recipient, content)
            VALUES (@IdSender, @IdRecipient, @Content)
            RETURNING id";

        var notificationId = connection.ExecuteScalar<Guid>(sql, new
        {
            notification.IdSender,
            notification.IdRecipient,
            notification.Content
        });

        return notificationId;
    }

    public IEnumerable<Notification> GetNotifications(Guid idRecipient)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var sql = @"
            SELECT 
                id AS Id,
                id_sender AS IdSender,
                id_recipient AS IdRecipient,
                content AS Content,
                is_read AS IsRead,
                created_at AS CreatedAt
            FROM notifications
            WHERE id_recipient = @IdRecipient
            ORDER BY created_at DESC";

        return connection.Query<Notification>(sql, new { IdRecipient = idRecipient });
    }

    public void PutNotificationAsRead(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var sql = @"
            UPDATE notifications
            SET is_read = true
            WHERE id = @Id";

        connection.Execute(sql, new { Id = id });
    }

    public Notification? GetNotification(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var sql = @"
            SELECT 
                id AS Id,
                id_sender AS IdSender,
                id_recipient AS IdRecipient,
                content AS Content,
                is_read AS IsRead,
                created_at AS CreatedAt
            FROM notifications
            WHERE id = @Id";

        return connection.QuerySingleOrDefault<Notification>(sql, new { Id = id });
    }
}