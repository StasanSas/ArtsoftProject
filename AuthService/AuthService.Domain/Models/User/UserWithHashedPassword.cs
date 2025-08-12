namespace AuthService.Domain.Models;

public class UserWithHashedPassword : User
{

    public UserWithHashedPassword(Guid id, string login, string password, bool isActive) : base(id, login, password, isActive)
    {
    }
}