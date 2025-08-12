namespace NotificationService.Domain;

public class NewNotification
{
    public Guid IdSender { get; private set; }
    
    public Guid IdRecipient { get; private set; }
    
    public string Content { get; private set; }
    
    public NewNotification(Guid idSender, Guid idRecipient, string content)
    {
        IdSender = idSender;
        IdRecipient = idRecipient;
        Content = content;
    }
}