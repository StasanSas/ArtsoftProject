using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthService.Application.CustomException;
using AuthService.Application.Interfaces;
using AuthService.Domain.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using ArgumentException = System.ArgumentException;
using UserInfo = AuthService.Domain.Models.UserInfo;

namespace AuthService.Application.Services;

public class AuthService : IAuthService
{
    private readonly RsaSecurityKey _privateKey;
    
    private readonly RsaSecurityKey _publicKey;
    

    public AuthService(RsaSecurityKey publicKey, RsaSecurityKey privateKey)
    {
        _privateKey = privateKey;
        _publicKey = publicKey;
    }

    public string GetJwtTokenWithInfoUser(UserInfo user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                _privateKey,
                SecurityAlgorithms.RsaSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);
        return jwt;
    }

    public string GetHashedPassword(NewUser user)
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        
        var hashedPassword = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: user.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 32 
            )
        );

        // Потом заиспользуем соль для проверки пароля, поэтому её тоже сохраняем
        var saltBase64 = Convert.ToBase64String(salt);
        
        return $"{hashedPassword}:{saltBase64}";
    }

    public bool IsCorrectPassword(UserWithHashedPassword userFromDb, string transmittedPassword)
    {
        // Разделяем хеш и соль
        string[] parts = userFromDb.Password.Split(':');
        if (parts.Length != 2)
            throw new ArgumentException("Not correct format password out database");

        var hashedPasswordWithoutSaltFromDb = parts[0];
        var salt = Convert.FromBase64String(parts[1]);

        // Хешируем введённый пароль с той же солью
        var transmittedPasswordHashed = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: transmittedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 32
            )
        );

        // Ура
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(hashedPasswordWithoutSaltFromDb),
            Convert.FromBase64String(transmittedPasswordHashed)
        );
    }

    public UserInfo GetUserInfoAboutUseByJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        
        var jwtToken = handler.ReadJwtToken(token);
        
        var identity = new ClaimsIdentity(jwtToken.Claims);
        var principal = new ClaimsPrincipal(identity);
            
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier);
        var username = principal.FindFirst(ClaimTypes.Name);
        
        if (userId == null)
            throw new NotCorrectFormatJwtToken("Not Found Claim NameIdentifier");
        if (username == null)
            throw new NotCorrectFormatJwtToken("Not Found Claim Name");

        Guid userIdInGuidFormat;
        var isPossibleParseInGuid = Guid.TryParse(userId.Value, out userIdInGuidFormat);
        if (!isPossibleParseInGuid)
            throw new NotCorrectFormatJwtToken("Not Valid Claim NameIdentifier");
        
        return new UserInfo(userIdInGuidFormat, username.Value);  
    }

    public string GetPublicKey()
    {
        using var rsa = RSA.Create();
        rsa.ImportParameters(_publicKey.Parameters);

        // Экспорт в PEM формате
        return "-----BEGIN PUBLIC KEY-----\n" + 
                           Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo(), Base64FormattingOptions.InsertLineBreaks) +
                           "\n-----END PUBLIC KEY-----";
        
    }
}