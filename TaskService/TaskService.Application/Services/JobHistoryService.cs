using TaskService.Application.Arguments;
using TaskService.Application.Interfaces;
using TaskService.Application.Interfaces.DbContexts;
using TaskService.Application.Interfaces.Services;
using TaskService.Domain;

namespace TaskService.Application.Services;

public class JobHistoryService : IJobHistoryService
{
    private IJobHistoryDbContext _jobHistoryDbContext;
    private IUserHttpService _userService;

    public JobHistoryService(IJobHistoryDbContext jobHistoryDbContext, IJobEventPublisher jobService, IUserHttpService userService)
    {
        this._jobHistoryDbContext = jobHistoryDbContext;
        jobService.Subscribe(SetHistoryJobEvent);
        this._userService = userService;
    }
    
    public void SetHistoryJobEvent(Guid id, EventType eventType)
    {
        var jobHistoryEvent = new NewJobHistoryEvent(id, eventType,  _userService.UserId);
        _jobHistoryDbContext.SetJobHistory(jobHistoryEvent);
    }

    public IEnumerable<JobHistoryEvent> GetHistoryJobEvent(Guid id)
    {
        return _jobHistoryDbContext.GetJobHistoryByJobId(id);
    }
}