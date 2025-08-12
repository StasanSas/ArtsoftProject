namespace AuthService.Persistence.Entity;

public class UserEntityWithId
{
    public Guid Id { get; set; }
    public string Login { get; set; }

    public string Password { get; set; }
    
    public bool IsActive { get; set; }
}