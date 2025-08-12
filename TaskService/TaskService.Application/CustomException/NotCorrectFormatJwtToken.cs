namespace TaskService.Application.CustomException;

public class NotCorrectFormatJwtToken: Exception
{
    public NotCorrectFormatJwtToken() { }

    public NotCorrectFormatJwtToken(string message) 
        : base(message) { }
}