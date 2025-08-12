using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services, RsaSecurityKey publicKey, RsaSecurityKey privateKey)
    {
        services.AddScoped<IUserService,UserService>();
        services.AddScoped<IAuthService>(_ => new Services.AuthService(publicKey, privateKey));
        return services;
    }
}