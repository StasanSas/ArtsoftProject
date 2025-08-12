using AuthService.Domain.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application.Interfaces;

public interface IAuthService
{
    public string GetJwtTokenWithInfoUser(UserInfo user);
    
    public UserInfo GetUserInfoAboutUseByJwt(string token);
    
    public string GetHashedPassword(NewUser user);

    public bool IsCorrectPassword(UserWithHashedPassword userFromDb, string transmittedPassword);
    
    public string GetPublicKey();
}