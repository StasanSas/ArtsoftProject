namespace AuthService.Persistence.Entity;

public class UserEntity
{
    public string Login { get; set; }

    public string Password { get; set; }
    
    public bool IsActive { get; set; }
}