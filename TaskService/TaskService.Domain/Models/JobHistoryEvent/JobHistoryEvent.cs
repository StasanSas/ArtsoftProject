namespace TaskService.Domain;

public enum EventType
{
    Created,
    Updated,
    Deleted,
    Assigned
}

public class JobHistoryEvent
{
    public Guid Id { get; private set; }
    public Guid JobId { get; private set; }
    public EventType EventType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    
    
    public JobHistoryEvent(Guid id, Guid jobId, EventType eventType, DateTime createdAt, Guid createdBy)
    {
        Id = id;
        JobId = jobId;
        EventType = eventType;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
    }
    
}