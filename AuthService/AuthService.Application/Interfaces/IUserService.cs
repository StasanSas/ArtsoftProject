using AuthService.Domain.Models;

namespace AuthService.Application.Interfaces;

public interface IUserService
{
    public void CreateUser(NewUserWithHashedPassword  user);

    public bool TryGetUserInDb(string loginUser, out UserWithHashedPassword  user);
}