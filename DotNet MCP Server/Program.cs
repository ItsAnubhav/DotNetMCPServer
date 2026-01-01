using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using System.ComponentModel;
using McpServerApp.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true);

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpHelper, HttpHelper>();
builder.Services.Configure<ExternalApisOptions>(builder.Configuration.GetSection("ExternalApis"));
builder.Services.AddSingleton<IExternalAuthService, ExternalAuthService>();

var app = builder.Build();

app.Run();

// Options and services
public record ExternalApisOptions
{
    public bool UseExternalAuth { get; init; }
    public string? FlightSearchBaseUrl { get; init; }
    public string? BookingDetailsBaseUrl { get; init; }
    public string? FareRulesBaseUrl { get; init; }
    public string? AuthEndpoint { get; init; }
    public string? AuthClientId { get; init; }
    public string? AuthClientSecret { get; init; }
}

public interface IExternalAuthService
{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}

public class ExternalAuthService : IExternalAuthService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ExternalApisOptions _opts;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _cachedToken;
    private DateTime _expiresAt = DateTime.MinValue;

    public ExternalAuthService(IHttpClientFactory httpFactory, IOptions<ExternalApisOptions> opts)
    {
        _httpFactory = httpFactory;
        _opts = opts.Value;
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

            var client = _httpFactory.CreateClient();
            var form = new Dictionary<string, string>
            {
                ["client_id"] = _opts.AuthClientId ?? string.Empty,
                ["client_secret"] = _opts.AuthClientSecret ?? string.Empty,
                ["grant_type"] = "client_credentials"
            };

            var resp = await client.PostAsync(_opts.AuthEndpoint, new FormUrlEncodedContent(form), cancellationToken);
            if (!resp.IsSuccessStatusCode)
                return null;

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

    private record AuthResponse(string access_token, int expires_in);
}
