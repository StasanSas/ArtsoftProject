namespace TaskService.Application.Interfaces.Services;

public interface IUserJwtTokenHttpService
{
    Guid UserId { get; }

    public string GetTokenFromRequest();
}