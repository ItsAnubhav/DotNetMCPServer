using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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
    private readonly IExternalAuthService _authService;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public HttpHelper(IHttpClientFactory factory, IExternalAuthService authService)
    {
        _factory = factory;
        _authService = authService;
    }

    public async Task<T?> GetJsonAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        var s = await GetStringAsync(url, cancellationToken);
        if (s is null) return default;
        return JsonSerializer.Deserialize<T>(s, _jsonOptions);
    }

    public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default)
    {
        var client = _factory.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        await ApplyAuthIfNeeded(req, cancellationToken);
        var resp = await client.SendAsync(req, cancellationToken);
        if (!resp.IsSuccessStatusCode) return null!;
        return await resp.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<TResponse?> PostJsonAsync<TRequest, TResponse>(string url, TRequest payload, CancellationToken cancellationToken = default)
    {
        var client = _factory.CreateClient();
        var json = JsonSerializer.Serialize(payload);
        var req = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        await ApplyAuthIfNeeded(req, cancellationToken);
        var resp = await client.SendAsync(req, cancellationToken);
        if (!resp.IsSuccessStatusCode) return default;
        var body = await resp.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<TResponse>(body, _jsonOptions);
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        return SendInternalAsync(request, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendInternalAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var client = _factory.CreateClient();
        await ApplyAuthIfNeeded(request, cancellationToken);
        return await client.SendAsync(request, cancellationToken);
    }

    private async Task ApplyAuthIfNeeded(HttpRequestMessage req, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _authService.GetTokenAsync(cancellationToken);
            if (!string.IsNullOrEmpty(token))
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        catch
        {
            // swallow auth errors; callers will get the HTTP failure
        }
    }
}
