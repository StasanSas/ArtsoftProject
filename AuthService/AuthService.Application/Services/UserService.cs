using AuthService.Application.CustomException;
using AuthService.Application.Interfaces;
using AuthService.Domain.Models;

namespace AuthService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserDbContext _userDbContext;
    
    public UserService(IUserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public Guid CreateUser(NewUserWithHashedPassword  user)
    {
        return _userDbContext.CreateUser(user);
    }

    public bool TryGetUserInDb(string loginUser, out UserWithHashedPassword user)
    {
        var isInDb = _userDbContext.ContainsUserInDb(loginUser);
        if (!isInDb)
        {
            user = null;
            return false;
        }
        user = _userDbContext.GetUserInDb(loginUser);
        if (user == null)
            throw new InternalDbException("User not found in db, but ContainsUserInDb return true");
        return true;
    }
}