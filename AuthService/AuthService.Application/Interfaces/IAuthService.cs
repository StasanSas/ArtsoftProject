using AuthService.Domain.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application.Interfaces;

public interface IAuthService
{
    public string GetJwtTokenWithInfoUser(UserInfo user);
    
    public UserInfo GetUserInfoAboutUseByJwt(string token);
    
    public string GetHashedPassword(User user);

    public bool IsCorrectPassword(User userFromDb, string transmittedPassword);
    
    public string GetPublicKey();
}