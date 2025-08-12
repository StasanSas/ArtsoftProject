using AuthService.Application;
using AuthService.Application.Interfaces;
using AuthService.Persistence.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddDb(
        this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IUserDbContext>(_ => new UserDbContext(connectionString));
        return services;
    }
}