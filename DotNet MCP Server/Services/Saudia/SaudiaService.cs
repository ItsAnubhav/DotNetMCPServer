using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using McpServerApp.Helpers;
using McpServerApp.Services.Saudia.Responses;

namespace McpServerApp.Services.Saudia;

public interface ISaudiaService
{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
    Task<OrderResponse?> GetOrderDetailsAsync(string pnr, string lastName, CancellationToken cancellationToken = default);
}

public class SaudiaService : ISaudiaService
{
    private readonly IHttpHelper _httpHelper;
    private readonly ILogger<SaudiaService> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _cachedToken;
    private DateTime _expiresAt = DateTime.MinValue;

    public SaudiaService(IHttpHelper httpHelper, ILogger<SaudiaService> logger)
    {
        _httpHelper = httpHelper;
        _logger = logger;
    }

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedToken is not null && DateTime.UtcNow < _expiresAt)
        {
            _logger.LogDebug("SaudiaService: returning cached token, expires at {ExpiresAt}", _expiresAt);
            return _cachedToken;
        }

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_cachedToken is not null && DateTime.UtcNow < _expiresAt)
                return _cachedToken;

            _logger.LogInformation("SaudiaService: requesting token from {AuthEndpoint}", SaudiaConstants.AuthEndpoint);
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

            var resp = await _httpHelper.SendAsync(req, cancellationToken);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("SaudiaService: token request failed with {StatusCode}", resp.StatusCode);
                return null;
            }

            var payload = await resp.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                _logger.LogWarning("SaudiaService: token response payload was null");
                return null;
            }

            _cachedToken = payload.access_token;
            _expiresAt = DateTime.UtcNow.AddSeconds(payload.expires_in > 60 ? payload.expires_in - 60 : payload.expires_in);
            _logger.LogInformation("SaudiaService: acquired token, expires at {ExpiresAt}", _expiresAt);
            return _cachedToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<OrderResponse?> GetOrderDetailsAsync(string pnr, string lastName, CancellationToken cancellationToken = default)
    {
        var url = $"{SaudiaConstants.host}/b2b/b2bportal/orders/{pnr}?lastName={lastName}&guestOfficeId={SaudiaConstants.guestOfficeId}";
        _logger.LogInformation("GetOrderDetailsAsync: fetching {Url}", url);
        var resp = await _httpHelper.GetJsonAsync<OrderResponse>(url, cancellationToken);
        if (resp is null)
            _logger.LogWarning("GetOrderDetailsAsync: failed to retrieve order {Pnr}", pnr);
        else
            _logger.LogDebug("GetOrderDetailsAsync: retrieved order {Pnr}", pnr);
        return resp;
    }

}
