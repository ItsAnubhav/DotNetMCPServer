using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using McpServerApp.Helpers;

namespace McpServerApp.Services;

public static class SaudiaConstants{

    public const string host = "http://dapi-uat.dcloud.saudia.com";

    public const string ClientId = "ddb05723-34db-40d1-8236-31f7379ef537";
    public const string ClientSecret = "12345678"; // Replace with actual secret
    public const string Scope = "259a88f9-c8c4-4083-9a20-a80d018987d7/.default";
    public const string Fact = "{\"keyValuePairs\":[{\"key\":\"flow\",\"value\":\"REVENUE\"},{\"key\":\"market\",\"value\":\"IND\"},{\"key\":\"originCity\",\"value\":\"JED\"},{\"key\":\"channel\",\"value\":\"DESKTOP\"}]}";

    public const string guestOfficeId = "LONSV08AA";

    public const string AuthEndpoint = $"{host}/session/auth/b2b/token/initialization";
}

public interface ISaudiaService{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}

public class SaudiaService : ISaudiaService
{
    private readonly IHttpHelper _httpHelper;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _cachedToken;
    private DateTime _expiresAt = DateTime.MinValue;

    public SaudiaService(IHttpHelper httpHelper)
    {
        _httpHelper = httpHelper;
    }

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!_opts.UseExternalAuth || string.IsNullOrEmpty(_opts.AuthEndpoint))
            return null;

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

            var resp = await _httpHelper.SendAsync(req, cancellationToken);
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