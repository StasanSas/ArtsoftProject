using Npgsql;
using Dapper;

public static class DbInitializer
{
    public static void Initialize(string connectionString)
    {
        var enableExtensionSql = "CREATE EXTENSION IF NOT EXISTS pgcrypto"; // иначе не генерится id

        var createTableJobSql = @"CREATE TABLE IF NOT EXISTS jobs (
            id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
            name VARCHAR(100) NOT NULL,
            description VARCHAR(500) NOT NULL
        )";

        var createTableJobExecutorsSql = @"CREATE TABLE IF NOT EXISTS job_executors (
            job_id UUID NOT NULL REFERENCES jobs(id) ON DELETE CASCADE,
            worker_id UUID NOT NULL,
            PRIMARY KEY (job_id, worker_id)
        )";
        
        var createTableTypeEvent = @"CREATE TABLE IF NOT EXISTS event_type (
            id INTEGER PRIMARY KEY,
            name VARCHAR(50) NOT NULL
        )";

        var fillTableRypeEvent = @"INSERT INTO event_type (id, name) VALUES 
            (0, 'Created'),
            (1, 'Updated'),
            (2, 'Deleted'),
            (3, 'Assigned')
            ON CONFLICT (id) DO NOTHING";

        var createTableHistoryEvent = @"CREATE TABLE IF NOT EXISTS job_history_event (
            id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
            job_id UUID NOT NULL REFERENCES jobs(id),
            event_type INTEGER NOT NULL REFERENCES event_type(id),
            created_at TIMESTAMP NOT NULL DEFAULT NOW(),
            created_by UUID NOT NULL
        )";

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using var transaction = connection.BeginTransaction();
        try
        {
            connection.Execute(enableExtensionSql, transaction: transaction);
            connection.Execute(createTableJobSql, transaction: transaction);
            connection.Execute(createTableJobExecutorsSql, transaction: transaction);
            connection.Execute(createTableTypeEvent, transaction: transaction);
            connection.Execute(fillTableRypeEvent, transaction: transaction);
            connection.Execute(createTableHistoryEvent, transaction: transaction);
            
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