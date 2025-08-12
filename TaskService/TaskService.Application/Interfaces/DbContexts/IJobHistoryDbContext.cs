using TaskService.Domain;

namespace TaskService.Application.Interfaces.DbContexts;

public interface IJobHistoryDbContext
{
    public void SetJobHistory(NewJobHistoryEvent jobHistoryEvent);
    
    public IEnumerable<JobHistoryEvent> GetJobHistoryByJobId(Guid id);
}