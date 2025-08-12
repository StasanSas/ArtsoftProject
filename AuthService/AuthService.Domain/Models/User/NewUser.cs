namespace AuthService.Domain.Models;

public class NewUser
{
    public string Login { get; private set; }

    public string Password { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public NewUser(string login, string password)
    {
        Login = login;
        Password = password;
        IsActive = true;
    }
    
}