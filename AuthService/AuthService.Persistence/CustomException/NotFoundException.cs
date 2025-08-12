namespace AuthService.Persistence.CustomException;

public class NotFoundException : Exception
{
    public NotFoundException() { }

    public NotFoundException(string message) 
        : base(message) { }
}