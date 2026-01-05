using McpServerApp.Services.Saudia;
using McpServerApp.Services.Saudia.Responses;

public interface IAuthService
{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _factory;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _cachedToken;
    private DateTime _expiresAt = DateTime.MinValue;

    public AuthService(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedToken is not null && DateTime.UtcNow < _expiresAt)
            return _cachedToken;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_cachedToken is not null && DateTime.UtcNow < _expiresAt)
                return _cachedToken;

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
            if (!resp.IsSuccessStatusCode) return null;

            var payload = await resp.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken);
            if (payload is null) return null;

            _cachedToken = payload.access_token;
            _expiresAt = DateTime.UtcNow.AddSeconds(payload.expires_in > 60 ? payload.expires_in - 60 : payload.expires_in);
            return _cachedToken;
        }
        finally
        {
            _lock.Release();
        }
    }
}