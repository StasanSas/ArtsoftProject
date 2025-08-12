using Dapper;
using Npgsql;
using TaskService.Application.Interfaces;
using TaskService.Application.Interfaces.DbContexts;
using TaskService.Domain;

namespace TaskService.Persistence.DbContexts;

public class JobHistoryDbContext : IJobHistoryDbContext
{
    private readonly string _connectionString;

    public JobHistoryDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void SetJobHistory(NewJobHistoryEvent jobHistoryEvent)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            const string sql = @"
                    INSERT INTO job_history_event (job_id, event_type, created_by)
                    VALUES (@JobId, @EventType, @CreatedBy)";

            var entity = new 
            {
                JobId = jobHistoryEvent.JobId,
                EventType = (int)jobHistoryEvent.EventType,
                CreatedBy = jobHistoryEvent.CreatedBy
            };

            var id = connection.ExecuteScalar<Guid>(sql, entity);
        }
    }

    public IEnumerable<JobHistoryEvent> GetJobHistoryByJobId(Guid jobId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            const string sql = @"
                    SELECT id, job_id AS JobId, event_type AS EventType, 
                           created_at AS CreatedAt, created_by AS CreatedBy
                    FROM job_history_event
                    WHERE job_id = @JobId
                    ORDER BY created_at DESC";

            var entities = connection.Query<JobHistoryEvent>(sql, new { JobId = jobId });

            return entities;
        }
    }
}