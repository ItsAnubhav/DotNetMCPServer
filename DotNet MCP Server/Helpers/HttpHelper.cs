using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using McpServerApp.Services;
using Microsoft.Extensions.Logging;

namespace McpServerApp.Helpers;

public interface IHttpHelper
{
    Task<T?> GetJsonAsync<T>(string url, CancellationToken cancellationToken = default);
    Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default);
    Task<TResponse?> PostJsonAsync<TRequest, TResponse>(string url, TRequest payload, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}

public class HttpHelper : IHttpHelper
{
    private readonly IHttpClientFactory _factory;
    private readonly IAuthService _authService;
    private readonly ILogger<HttpHelper> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public HttpHelper(IHttpClientFactory factory, IAuthService authService, ILogger<HttpHelper> logger)
    {
        _factory = factory;
        _authService = authService;
        _logger = logger;
    }

    public async Task<T?> GetJsonAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GetJsonAsync: {Url}", url);
        var s = await GetStringAsync(url, cancellationToken);
        if (s is null) return default;
        return JsonSerializer.Deserialize<T>(s, _jsonOptions);
    }

    public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("GetStringAsync GET {Url}", url);
        var client = _factory.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        await ApplyAuthIfNeeded(req, cancellationToken);
        var resp = await client.SendAsync(req, cancellationToken);
        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogWarning("GET {Url} returned {StatusCode}", url, resp.StatusCode);
            return null!;
        }
        var body = await resp.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogDebug("GET {Url} succeeded ({Length} bytes)", url, body?.Length ?? 0);
        return body;
    }

    public async Task<TResponse?> PostJsonAsync<TRequest, TResponse>(string url, TRequest payload, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("PostJsonAsync POST {Url}", url);
        var client = _factory.CreateClient();
        var json = JsonSerializer.Serialize(payload);
        var req = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        await ApplyAuthIfNeeded(req, cancellationToken);
        var resp = await client.SendAsync(req, cancellationToken);
        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogWarning("POST {Url} returned {StatusCode}", url, resp.StatusCode);
            return default;
        }
        var body = await resp.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogDebug("POST {Url} succeeded ({Length} bytes)", url, body?.Length ?? 0);
        return JsonSerializer.Deserialize<TResponse>(body, _jsonOptions);
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        return SendInternalAsync(request, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendInternalAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("SendInternalAsync {Method} {Uri}", request.Method, request.RequestUri);
        var client = _factory.CreateClient();
        await ApplyAuthIfNeeded(request, cancellationToken);
        var resp = await client.SendAsync(request, cancellationToken);
        _logger.LogDebug("Response {StatusCode} for {Method} {Uri}", resp.StatusCode, request.Method, request.RequestUri);
        return resp;
    }

    private async Task ApplyAuthIfNeeded(HttpRequestMessage req, CancellationToken cancellationToken)
    {
        try
        {
            // Allow callers to opt-out of automatic auth (useful for acquiring the token itself).
            if (req.Headers.Contains("X-Skip-Auth"))
            {
                _logger.LogDebug("Skipping auth for {Uri}", req.RequestUri);
                return;
            }

            var token = await _authService.GetTokenAsync(cancellationToken);
            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogDebug("Applying bearer token to request {Uri}", req.RequestUri);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogDebug("No token available for request {Uri}", req.RequestUri);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to apply auth to request {Uri}", req.RequestUri);
            // swallow auth errors; callers will get the HTTP failure
        }
    }
}
