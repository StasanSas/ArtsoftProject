using TaskService.Application.Arguments;
using TaskService.Domain;

namespace TaskService.Application.Interfaces.Services;

public interface IJobService
{
    public IEnumerable<Workflow> GetAllJobs(GetJobsArgument argument);

    public Workflow GetJob(Guid id);

    public Guid CreateJob(NewJob job);

    public void UpdateJob(Job job);

    public void DeleteJob(Guid id);

    public void AssignExecutor(Guid jobId, Guid executorId);
}