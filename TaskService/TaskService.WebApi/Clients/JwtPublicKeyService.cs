using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace TaskService.WebApi.Clients;

public class JwtPublicKeyService
{

    private HttpClient _httpClient { get; set; }
    
    private RsaSecurityKey _publicKey { get; set; }

    private bool IsLoadedKey { get; set; }

    public JwtPublicKeyService(
        IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("jwt");
        IsLoadedKey = true;
        GetPublicKey();
    }

    public async Task<RsaSecurityKey> GetPublicKey()
    {
        if (!IsLoadedKey)
        {
            var response = await _httpClient.GetAsync("api/auth/key");
            response.EnsureSuccessStatusCode();
            var publicKeyInString = await response.Content.ReadAsStringAsync();
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyInString);
            _publicKey = new RsaSecurityKey(rsa);
            IsLoadedKey = true;
        }


        return _publicKey;
    }
}