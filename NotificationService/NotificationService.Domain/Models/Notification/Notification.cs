namespace NotificationService.Domain;

public class Notification
{
    public Guid Id { get; private set; }
    
    public Guid IdSender { get; private set; }
    
    public Guid IdRecipient { get; private set; }
    
    public string Content { get; private set; }
    
    public bool IsRead { get; private set; }
    
    public DateTime CreatedAt { get; private set; } 
    
    public Notification(Guid id, Guid idSender, Guid idRecipient, string content, bool isRead, DateTime createdAt)
    {
        Id = id;
        IdSender = idSender;
        IdRecipient = idRecipient;
        Content = content;
        IsRead = isRead;
        CreatedAt = createdAt;
    }

    public void Read()
    {
        IsRead = true;
    }

}