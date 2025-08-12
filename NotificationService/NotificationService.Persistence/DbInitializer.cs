using Npgsql;
using Dapper;

public static class DbInitializer
{
    public static void Initialize(string connectionString)
    {
        var enableExtensionSql = "CREATE EXTENSION IF NOT EXISTS pgcrypto"; // иначе не генерится id
        
        
        var createTableNotification = @"CREATE TABLE IF NOT EXISTS notifications (
            id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
            id_sender UUID NOT NULL,
            id_recipient UUID NOT NULL,
            content TEXT NOT NULL,
            is_read BOOLEAN NOT NULL DEFAULT FALSE,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
        )";


        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using var transaction = connection.BeginTransaction();
        try
        {
            connection.Execute(enableExtensionSql, transaction: transaction);
            connection.Execute(createTableNotification, transaction: transaction);
            
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"Database initialization failed: {ex.Message}");
            throw;
        }
    }
}