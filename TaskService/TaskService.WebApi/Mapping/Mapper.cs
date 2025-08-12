using TaskService.Domain;
using TaskService.WebApi.Dto;

namespace TaskService.WebApi.Mapping;

public static class Mapper
{
    public static JobHistoryEventDto ToJobHistoryEventDto(this JobHistoryEvent jobHistoryEvent)
    {
        return new JobHistoryEventDto()
        {
            CreatedAt = jobHistoryEvent.CreatedAt,
            CreatedBy = Guid.NewGuid(),
            EventType = jobHistoryEvent.EventType,
            Id = jobHistoryEvent.Id,
            JobId = jobHistoryEvent.JobId
        };
    }
    
    public static WorkflowDto ToWorkflowDto(this Workflow workflow)
    {
        return new WorkflowDto()
        {
            Id = workflow.job.Id,
            Name = workflow.job.Name,
            Description = workflow.job.Description,
            Executors = workflow.Executors.Select(ex => ex.Id)
        };
    }
}