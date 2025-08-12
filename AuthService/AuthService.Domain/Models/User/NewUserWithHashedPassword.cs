namespace AuthService.Domain.Models;

public class NewUserWithHashedPassword : NewUser
{
    public NewUserWithHashedPassword(string login, string password) : base(login, password)
    {
    }
}