using System.Net.Http.Headers;
using TaskService.Application.Interfaces.Services;

namespace TaskService.WebApi.Clients.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IUserJwtTokenHttpService _tokenService;

    public AuthHeaderHandler(IUserJwtTokenHttpService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenService.GetTokenFromRequest(); 
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}