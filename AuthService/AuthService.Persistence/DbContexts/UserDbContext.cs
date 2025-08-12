using AuthService.Application.Interfaces;
using AuthService.Domain.Models;
using AuthService.Persistence.CustomException;
using Dapper;
using Npgsql;

namespace AuthService.Persistence.DbContexts;

public class UserDbContext : IUserDbContext
{
    
    private readonly string _connectionString;

    public UserDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public Guid CreateUser(NewUserWithHashedPassword user)
        {
            const string sql = @"
                INSERT INTO users (login, password_hash, is_active)
                VALUES (@Login, @Password, @IsActive)
                RETURNING user_id";
            
            using var connection = new NpgsqlConnection(_connectionString);
            
            var userId = connection.ExecuteScalar<Guid>(sql, new NewUser(user.Login, user.Password));
            return userId;
        }

        public bool ContainsUserInDb(string loginUser)
        {
            const string sql = "SELECT COUNT(1) FROM users WHERE login = @Login";
            
            using var connection = new NpgsqlConnection(_connectionString);
            
            return connection.ExecuteScalar<int>(sql, new { Login = loginUser }) > 0;
        }

        public UserWithHashedPassword? GetUserInDb(string loginUser)
        {
            const string sql = @"
                SELECT 
                    user_id as Id,
                    login as Login,
                    password_hash as Password,
                    is_active as IsActive
                FROM users 
                WHERE login = @Login";
            
            using var connection = new NpgsqlConnection(_connectionString);
            
            var userEntity = connection.QueryFirstOrDefault<UserWithHashedPassword>(sql, new { Login = loginUser });

            return userEntity;
        }
}