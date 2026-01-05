using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using McpServerApp.Helpers;
using McpServerApp.Services.Saudia;
using McpServerApp.Services.Saudia.Responses;

public interface IAuthService
{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<AuthService> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _cachedToken;
    private DateTime _expiresAt = DateTime.MinValue;

    public AuthService(IHttpClientFactory factory, ILogger<AuthService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
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

            _logger.LogInformation("AuthService: requesting token from {AuthEndpoint}", SaudiaConstants.AuthEndpoint);
            var req = new HttpRequestMessage(HttpMethod.Post, SaudiaConstants.AuthEndpoint);

            // Build form content using SaudiaConstants
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = SaudiaConstants.ClientId,
                ["client_secret"] = SaudiaConstants.ClientSecret,
                ["scope"] = SaudiaConstants.Scope,
                ["fact"] = SaudiaConstants.Fact
            };

            req.Content = new FormUrlEncodedContent(form);
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
}