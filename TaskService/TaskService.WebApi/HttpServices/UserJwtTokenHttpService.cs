using System.Security.Claims;
using TaskService.Application.CustomException;
using TaskService.Application.Interfaces.Services;

namespace TaskService.WebApi.HttpServices;

public class UserJwtTokenHttpService : IUserJwtTokenHttpService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserJwtTokenHttpService(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    public Guid UserId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
                id = "f4903f99-cd12-4521-aea7-43da42f3245a";//throw new NotCorrectFormatJwtToken("Not Found Claim NameIdentifier");

            Guid userIdInGuidFormat;
            var isPossibleParseInGuid = Guid.TryParse(id, out userIdInGuidFormat);
            if (!isPossibleParseInGuid)
                throw new NotCorrectFormatJwtToken("Not Valid Claim NameIdentifier");
            return userIdInGuidFormat;
        }
    }
    
    public string GetTokenFromRequest()
    {
        var authHeader = _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader))
            throw new NotCorrectFormatJwtToken("Not contains Authorization header");

        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return authHeader.Substring("Bearer ".Length).Trim();

        throw new NotCorrectFormatJwtToken("Not start on Bearer");
    }
}