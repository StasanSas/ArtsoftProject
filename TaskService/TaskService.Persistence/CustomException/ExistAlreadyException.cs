namespace TaskService.Persistence.CustomException;

public class ExistAlreadyException : Exception
{
    public ExistAlreadyException() { }

    public ExistAlreadyException(string message) 
        : base(message) { }

    public ExistAlreadyException(string message, Exception inner) 
        : base(message, inner) { }
}