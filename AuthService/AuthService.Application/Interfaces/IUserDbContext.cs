using System;
using AuthService.Domain.Models;

namespace AuthService.Application.Interfaces;

public interface IUserDbContext
{
    public Guid CreateUser(NewUserWithHashedPassword  user);

    public bool ContainsUserInDb(string loginUser);
    
    public UserWithHashedPassword GetUserInDb(string loginUser);
}