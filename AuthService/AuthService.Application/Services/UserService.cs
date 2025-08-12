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

    public void CreateUser(UserWithHashedPassword  user)
    {
        _userDbContext.CreateUser(user);
    }

    public bool TryGetUserInDb(string loginUser, out UserWithHashedPassword user)
    {
        user = null;
        var isInDb = _userDbContext.ContainsUserInDb(loginUser);
        if (!isInDb)
            return false;
        user = _userDbContext.GetUserInDb(loginUser);
        return true;
    }
}