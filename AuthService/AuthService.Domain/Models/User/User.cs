using System;

namespace AuthService.Domain.Models;

public class User
{
    
    public Guid Id { get; private set; }
    public string Login { get; private set; }

    public string Password { get; private set; }
    
    public bool IsActive { get; private set; }
    
    
    public User(Guid id, string login, string password, bool isActive)
    {
        Id = id;
        Login = login;
        Password = password;
        IsActive = isActive;
    }

    public UserInfo GetInfoAboutMe()
    {
        var userInfo = new UserInfo(Id, Login);
        return userInfo;
    }
}