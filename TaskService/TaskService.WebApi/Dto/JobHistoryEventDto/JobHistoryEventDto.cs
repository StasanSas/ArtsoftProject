using TaskService.Domain;

namespace TaskService.WebApi.Dto;

public class JobHistoryEventDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public EventType EventType { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    
}