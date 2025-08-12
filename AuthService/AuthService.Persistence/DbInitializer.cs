using Dapper;
using Npgsql;

namespace AuthService.Persistence;

public class DbInitializer
{
    public static void Initialize(string connectionString)
    {
        var enableExtensionSql = "CREATE EXTENSION IF NOT EXISTS pgcrypto"; // иначе не генерится id
        
        var createTableUsers = @"
                CREATE TABLE IF NOT EXISTS users (
                    user_id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- GUID
                    login VARCHAR(100) UNIQUE NOT NULL,
                    password_hash VARCHAR(255) NOT NULL,
                    is_active BOOLEAN DEFAULT TRUE
                )";

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            connection.Execute(enableExtensionSql, transaction: transaction);
            connection.Execute(createTableUsers, transaction: transaction);
            
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