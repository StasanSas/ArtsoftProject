namespace AuthService.Domain.Models;

public class UserWithHashedPassword : User
{
    public UserWithHashedPassword(string login, string password) : base(login, password)
    {
    }

    public UserWithHashedPassword(Guid id, string login, string password) : base(id, login, password)
    {
    }
}