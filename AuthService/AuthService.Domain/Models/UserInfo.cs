namespace AuthService.Domain.Models;

public class UserInfo
{
    public Guid Id { get; private set; }
    public string Login { get; private set; }
    
    public UserInfo(Guid id, string login)
    {
        Id = id;
        Login = login;
    }
}