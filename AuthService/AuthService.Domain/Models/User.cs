namespace AuthService.Domain.Models;

public class User
{
    
    public Guid? Id { get; private set; }
    public string Login { get; private set; }

    public string Password { get; private set; }
    
    public User(string login, string password)
    {
        Login = login;
        Password = password;
    }
    
    public User(Guid id, string login, string password)
    :this(login, password)
    {
        Id = id;
    }

    public UserInfo GetInfoAboutMe()
    {
        if (!Id.HasValue)
            throw new ArgumentNullException(nameof(Id), "Id cannot be null or empty.");
        var userInfo = new UserInfo(Id.Value, Login);
        return userInfo;
    }
}