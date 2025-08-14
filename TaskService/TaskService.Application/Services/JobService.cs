using TaskService.Application.Arguments;
using TaskService.Application.CustomException;
using TaskService.Application.Interfaces;
using TaskService.Application.Interfaces.DbContexts;
using TaskService.Application.Interfaces.Services;
using TaskService.Domain;
using TaskService.Persistence.CustomException;

namespace TaskService.Application.Services;

public class JobService : IJobService, IJobEventPublisher
{
    private IJobDbContext _jobDbContext;
    
    private INotificationSenderService _notificationSenderService;
    
    private IUserJwtTokenHttpService _userJwtTokenService;

    private event Action<Guid, EventType>  handleEventJob;

    public JobService(IJobDbContext jobDbContext, INotificationSenderService notificationSenderService, IUserJwtTokenHttpService userJwtTokenService)
    {
        _jobDbContext = jobDbContext;
        _notificationSenderService = notificationSenderService;
        this._userJwtTokenService = userJwtTokenService;
    }

    public void Subscribe(Action<Guid, EventType> metod)
    {
        handleEventJob += metod;
    }
    
    public IEnumerable<Workflow> GetAllJobs(GetJobsArgument argument)
    {
        return _jobDbContext.GetWorkflows(argument);
    }
    
    public Workflow GetJob(Guid id)
    {
        var workflow = _jobDbContext.GetWorkflowById(id);
        if (workflow == null)
            throw new NotFoundException("Job not found");
        return workflow;
    }
    
    public Guid CreateJob(NewJob job)
    {
        var id = _jobDbContext.CreateJob(job);
        handleEventJob?.Invoke(id, EventType.Created);
        return id;
    }
    
    public void UpdateJob(Job job)
    {
        if (!_jobDbContext.JobExists(job.Id))
            throw new NotFoundException("Job not found");
        _jobDbContext.UpdateJob(job);
        handleEventJob?.Invoke(job.Id, EventType.Updated);
    }
    
    public void DeleteJob(Guid id)
    {
        if (!_jobDbContext.JobExists(id))
            throw new NotFoundException("Job not found");
        _jobDbContext.DeleteJob(id);
        handleEventJob?.Invoke(id, EventType.Deleted);
    }
    
    public void AssignExecutor(Guid jobId, Guid executorId)
    {
        if (!_jobDbContext.JobExists(jobId))
            throw new NotFoundException("Job not found");
        
        //if (!_jobDbContext.ExecutorExists(executorId))
            //throw new NotFoundException("Executor not found");
        _jobDbContext.AssignExecutor(jobId, executorId);
        var workflow = _jobDbContext.GetWorkflowById(jobId);
        if (workflow == null)
            throw new InternalDbException("Not get job, but say what this exist");
        var newNotification = new NewNotification(
            _userJwtTokenService.UserId,
            executorId,
            $"Необходимо выполнить работу с названием: {workflow.job.Name}\n" +
            $"Подробное описание: {workflow.job.Description}");
        _notificationSenderService.SendNotification(newNotification);
        handleEventJob?.Invoke(jobId, EventType.Assigned);
    }
    
}