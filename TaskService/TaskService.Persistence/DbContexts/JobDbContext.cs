using System.Data;
using Dapper;
using Npgsql;
using TaskService.Application.Arguments;
using TaskService.Application.Interfaces;
using TaskService.Application.Interfaces.DbContexts;
using TaskService.Domain;
using TaskService.Persistence.CustomException;

namespace TaskService.Persistence.DbContexts;

public class JobDbContext : IJobDbContext
{
    private readonly string _connectionString;
    public JobDbContext(string connectionString)
    {
        this._connectionString = connectionString;
    }    
    
    public IEnumerable<Workflow> GetWorkflows(GetJobsArgument args)
    {
        var sql = @"
        SELECT 
        j.Id, j.Name, j.Description, j.is_deleted As IsDeleted, 
        je.worker_id AS ExecutorId
    FROM 
        jobs j
    LEFT JOIN 
        job_executors je ON j.Id = je.job_id AND je.is_deleted = FALSE
    WHERE
        j.is_deleted = FALSE
        AND (@startName IS NULL OR j.Name LIKE @startName || '%')
    ORDER BY 
        j.Name
    OFFSET 
        @Offset ROWS 
    FETCH NEXT 
        @PageSize ROWS ONLY";

        var parameters = new {
            startName = args.startName ?? "",
            Offset = ((args.page - 1) ?? 0) * (args.pageSize ?? 10),
            PageSize = args.pageSize ?? 10
        };
        
        var jobDict = new Dictionary<Guid, Workflow>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var jobs = connection.Query<Job, Guid?, Workflow>(
                sql,
                (job, executorId) => 
                {
                    if (!jobDict.TryGetValue(job.Id, out var workflow))
                    {
                        workflow = new Workflow(job, new List<Executor>());
                        jobDict.Add(workflow.job.Id, workflow);
                    }
                    
                    if (executorId.HasValue && !workflow.Contains(executorId.Value))
                    {
                        workflow.AddExecutor(new Executor(executorId.Value));
                    }
        
                    return workflow;
                },
                parameters,
                splitOn: "ExecutorId"  
            ).AsList();  
    
            return jobDict.Values;
        }
    }

    public Workflow? GetWorkflowById(Guid id)
    {
        const string sql = @"
    SELECT 
        j.Id, j.Name, j.Description, j.is_deleted As IsDeleted,
        je.worker_id AS ExecutorId
    FROM 
        jobs j
    LEFT JOIN 
        job_executors je ON j.Id = je.job_id AND je.is_deleted = FALSE
    WHERE 
        j.Id = @id AND j.is_deleted = FALSE";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var jobDict = new Dictionary<Guid, Workflow>();
            
            var jobs = connection.Query<Job, Guid?, Workflow>(
                sql,
                (job, executorId) => 
                {
                    if (!jobDict.TryGetValue(job.Id, out var workflow))
                    {
                        workflow = new Workflow(job, new List<Executor>());
                        jobDict.Add(workflow.job.Id, workflow);
                    }
                    
                    if (executorId.HasValue && !workflow.Contains(executorId.Value))
                    {
                        workflow.AddExecutor(new Executor(executorId.Value));
                    }
        
                    return workflow;
                },
                new { id },
                splitOn: "ExecutorId");
                
            return jobDict.Values.FirstOrDefault();
        }
    }

    public Guid CreateJob(NewJob job)
    {
        const string insertJobSql =@"
        INSERT INTO jobs (Name, Description)
        VALUES (@Name, @Description)
        RETURNING Id";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var id = connection.ExecuteScalar<Guid>(insertJobSql, new 
            {
                job.Name,
                job.Description
            });
            return id;
        }
    }

    public void UpdateJob(Job job)
    {
        const string updateJobSql = @"
        UPDATE jobs 
        SET Name = @Name, 
            Description = @Description
        WHERE Id = @Id AND is_deleted = FALSE";

        using (var connection = new NpgsqlConnection(_connectionString)){
            connection.Execute(updateJobSql, new
            {
                job.Id,
                job.Name,
                job.Description
            });
        }
    }

    public void DeleteJob(Guid id)
    {
        const string sqlDeleteJob = @"UPDATE jobs SET is_deleted = TRUE WHERE id = @id;";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Execute(sqlDeleteJob, new { Id = id });
        }
    }

    public void AssignExecutor(Guid jobId, Guid executorId)
    {
        const string sql = @"
        INSERT INTO job_executors (job_id, worker_id)
        VALUES (@JobId, @WorkerId)
        ON CONFLICT (job_id, worker_id) DO NOTHING";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var result = connection.Execute(sql, new 
            {
                JobId = jobId,
                WorkerId = executorId
            });
            
            if (result == 0)
            {
                throw new ExistAlreadyException(
                    $"Assignment already exists for job {jobId} and executor {executorId}");
            }
            
        }
    }

    public bool JobExists(Guid jobId)
    {
        const string sql = "SELECT 1 FROM jobs WHERE Id = @id AND is_deleted = FALSE";
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            return connection.ExecuteScalar<bool>(sql, new { id = jobId });
        }
    }
    

    public void DeleteExecutor(Job job, Executor executor)
    {
        throw new NotImplementedException();
    }
}