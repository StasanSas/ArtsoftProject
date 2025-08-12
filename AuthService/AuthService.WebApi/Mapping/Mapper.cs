using AuthService.Domain.Models;
using AuthService.WebApi.Dto;

namespace TaskService.WebApi.Mapping;

public static class Mapper
{
    public static UserInfoDto ToUserInfoDto(this UserInfo user)
    {
        return new UserInfoDto()
        {
            Id = user.Id.ToString(),
            Login = user.Login
        };
    }
    
}