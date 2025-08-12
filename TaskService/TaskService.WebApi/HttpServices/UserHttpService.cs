using System.Security.Claims;
using TaskService.Application.CustomException;
using TaskService.Application.Interfaces.Services;

namespace TaskService.WebApi.HttpServices;

public class UserHttpService : IUserHttpService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserHttpService(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    public Guid UserId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
                throw new NotCorrectFormatJwtToken("Not Found Claim NameIdentifier");

            Guid userIdInGuidFormat;
            var isPossibleParseInGuid = Guid.TryParse(id, out userIdInGuidFormat);
            if (!isPossibleParseInGuid)
                throw new NotCorrectFormatJwtToken("Not Valid Claim NameIdentifier");
            return userIdInGuidFormat;
        }
    }
}