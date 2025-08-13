using TaskService.Application.Arguments;
using TaskService.Application.Interfaces;
using TaskService.Application.Interfaces.DbContexts;
using TaskService.Application.Interfaces.Services;
using TaskService.Domain;

namespace TaskService.Application.Services;

public class JobHistoryService : IJobHistoryService
{
    private IJobHistoryDbContext _jobHistoryDbContext;
    private IUserJwtTokenHttpService _userJwtTokenService;

    public JobHistoryService(IJobHistoryDbContext jobHistoryDbContext, IJobEventPublisher jobService, IUserJwtTokenHttpService userJwtTokenService)
    {
        this._jobHistoryDbContext = jobHistoryDbContext;
        jobService.Subscribe(SetHistoryJobEvent);
        this._userJwtTokenService = userJwtTokenService;
    }
    
    public void SetHistoryJobEvent(Guid id, EventType eventType)
    {
        var jobHistoryEvent = new NewJobHistoryEvent(id, eventType,  _userJwtTokenService.UserId);
        _jobHistoryDbContext.SetJobHistory(jobHistoryEvent);
    }

    public IEnumerable<JobHistoryEvent> GetHistoryJobEvent(Guid id)
    {
        return _jobHistoryDbContext.GetJobHistoryByJobId(id);
    }
}