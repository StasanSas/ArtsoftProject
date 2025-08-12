using TaskService.Domain;

namespace TaskService.Application.Interfaces.Services;

public interface IJobHistoryService
{
    public IEnumerable<JobHistoryEvent> GetHistoryJobEvent(Guid id);
}