using TaskService.Application.Arguments;
using TaskService.Domain;

namespace TaskService.Application.Interfaces.DbContexts;

public interface IJobDbContext
{
    public IEnumerable<Workflow> GetWorkflows(GetJobsArgument argument);
    
    public Workflow? GetWorkflowById(Guid id);
    
    public Guid CreateJob(NewJob job);
    
    public void UpdateJob(Job job);
    
    public void DeleteJob(Guid id);

    public void AssignExecutor(Guid jobId, Guid executorId);

    public bool JobExists(Guid jobId);

    public bool ExecutorExists(Guid executorId);

}