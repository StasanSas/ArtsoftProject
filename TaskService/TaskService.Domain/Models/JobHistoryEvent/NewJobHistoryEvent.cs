namespace TaskService.Domain;

public class NewJobHistoryEvent
{
    public Guid JobId { get; private set; }
    public EventType EventType { get; private set; }
    public Guid CreatedBy { get; private set; }
    
    public NewJobHistoryEvent(Guid jobId, EventType eventType, Guid createdBy)
    {
        JobId = jobId;
        EventType = eventType;
        CreatedBy = createdBy;
    }
}