using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using McpServerApp.Helpers;
using McpServerApp.Services.Saudia;
using McpServerApp.Services.Saudia.Responses;
using McpServerApp.Services.Travog.Request;
using McpServerApp.Services.Travog.Response;

namespace McpServerApp.Services.Travog;

public interface IQLAuthService
{
    Task<string?> GetTokenAsync(QLAuthRequest authRequest,CancellationToken cancellationToken = default);
    Task<DataTokenResponse?> GetDataTokenAsync(string companyId, string userName, string password, CancellationToken cancellationToken = default);
}

public class QLAuthService : IQLAuthService
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<QLAuthService> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _cachedToken;
    private DateTime _expiresAt = DateTime.MinValue;

    public QLAuthService(IHttpClientFactory factory, ILogger<QLAuthService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<string?> GetTokenAsync(QLAuthRequest authRequest, CancellationToken cancellationToken = default)
    {
        if (_cachedToken is not null && DateTime.UtcNow < _expiresAt)
        {
            _logger.LogDebug("AuthService: returning cached token, expires at {ExpiresAt}", _expiresAt);
            return _cachedToken;
        }

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_cachedToken is not null && DateTime.UtcNow < _expiresAt)
                return _cachedToken;

            _logger.LogInformation("AuthService: requesting token from {AuthEndpoint}", TravogConstants.QLAuthEndpoint);
            var req = new HttpRequestMessage(HttpMethod.Post, TravogConstants.QLAuthEndpoint);
            // Use the provided authRequest as JSON body for the token request
            req.Content = JsonContent.Create(authRequest);
            // tell HttpHelper to skip applying the bearer token for this request
            req.Headers.Add("X-Skip-Auth", "1");

            var client = _factory.CreateClient();
            var resp = await client.SendAsync(req, cancellationToken);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("AuthService: token request failed with {StatusCode}", resp.StatusCode);
                return null;
            }

            var payload = await resp.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                _logger.LogWarning("AuthService: token response payload was null");
                return null;
            }

            _cachedToken = payload.access_token;
            _expiresAt = DateTime.UtcNow.AddSeconds(payload.expires_in > 60 ? payload.expires_in - 60 : payload.expires_in);
            _logger.LogInformation("AuthService: acquired token, expires at {ExpiresAt}", _expiresAt);
            return _cachedToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<DataTokenResponse?> GetDataTokenAsync(string companyId, string userName, string password, CancellationToken cancellationToken = default)
    {
        // curl --location 'https://preprod.quadlabs.net/XChangeauth/api/auth/generateToken' \
        // --header 'accept: */*' \
        // --header 'Content-Type: application/json' \
        // --data-raw '{
        // "companyId": "QLabs12345",
        // "userName": "sa",
        // "password": "Qu@d1@bs"
        // }'

        var url = "https://preprod.quadlabs.net/XChangeauth/api/auth/generateToken";
        var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Headers.Accept.ParseAdd("*/*");
        var body = new
        {
            companyId,
            userName,
            password
        };
        req.Content = JsonContent.Create(body);
        req.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        var client = _factory.CreateClient();
        HttpResponseMessage resp;
        try
        {
            resp = await client.SendAsync(req, cancellationToken);
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("GetDataTokenAsync cancelled for companyId={CompanyId}", companyId);
            throw;
        }

        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetDataTokenAsync: upstream returned {StatusCode} for companyId={CompanyId}", resp.StatusCode, companyId);
            return null;
        }

        var payload = await resp.Content.ReadFromJsonAsync<RootDataTokenResponse>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            _logger.LogWarning("GetDataTokenAsync: response payload was null for companyId={CompanyId}", companyId);
            return null;
        }

        _logger.LogInformation("GetDataTokenAsync: acquired token for companyId={CompanyId}", companyId);
        return payload.data;
    }

}