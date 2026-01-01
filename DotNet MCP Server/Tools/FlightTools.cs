using ModelContextProtocol.Server;
using System.ComponentModel;
using Microsoft.Extensions.Options;

namespace McpServerApp.Tools;

[McpServerToolType]
public static class FlightTools
{
    [McpServerTool]
    [Description("Search available flights between origin and destination on a date.")]
    public static async Task<string> FlightSearch(
        HttpClient httpClient,
        IExternalAuthService authService,
        IOptions<ExternalApisOptions> opts,
        [Description("IATA origin code")] string origin,
        [Description("IATA destination code")] string destination,
        [Description("Date in YYYY-MM-DD")] string date,
        CancellationToken cancellationToken = default)
    {
        var options = opts.Value;
        if (string.IsNullOrEmpty(options.FlightSearchBaseUrl))
            return "Flight search base URL not configured.";

        var url = $"{options.FlightSearchBaseUrl.TrimEnd('/')}/search?origin={origin}&destination={destination}&date={date}";
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        var token = await authService.GetTokenAsync(cancellationToken);
        if (!string.IsNullOrEmpty(token)) req.Headers.Authorization = new("Bearer", token);

        var resp = await httpClient.SendAsync(req, cancellationToken);
        if (!resp.IsSuccessStatusCode)
            return $"Flight search request failed: {resp.StatusCode}";

        var body = await resp.Content.ReadAsStringAsync(cancellationToken);
        return body;
    }

    [McpServerTool]
    [Description("Get booking details by booking id.")]
    public static async Task<string> BookingDetails(
        HttpClient httpClient,
        IExternalAuthService authService,
        IOptions<ExternalApisOptions> opts,
        [Description("Booking identifier")] string bookingId,
        CancellationToken cancellationToken = default)
    {
        var options = opts.Value;
        if (string.IsNullOrEmpty(options.BookingDetailsBaseUrl))
            return "Booking details base URL not configured.";

        var url = $"{options.BookingDetailsBaseUrl.TrimEnd('/')}/bookings/{bookingId}";
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        var token = await authService.GetTokenAsync(cancellationToken);
        if (!string.IsNullOrEmpty(token)) req.Headers.Authorization = new("Bearer", token);

        var resp = await httpClient.SendAsync(req, cancellationToken);
        if (!resp.IsSuccessStatusCode)
            return $"Booking details request failed: {resp.StatusCode}";

        return await resp.Content.ReadAsStringAsync(cancellationToken);
    }

    [McpServerTool]
    [Description("Fetch fare rules by fare id.")]
    public static async Task<string> FareRules(
        HttpClient httpClient,
        IExternalAuthService authService,
        IOptions<ExternalApisOptions> opts,
        [Description("Fare identifier")] string fareId,
        CancellationToken cancellationToken = default)
    {
        var options = opts.Value;
        if (string.IsNullOrEmpty(options.FareRulesBaseUrl))
            return "Fare rules base URL not configured.";

        var url = $"{options.FareRulesBaseUrl.TrimEnd('/')}/fares/{fareId}/rules";
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        var token = await authService.GetTokenAsync(cancellationToken);
        if (!string.IsNullOrEmpty(token)) req.Headers.Authorization = new("Bearer", token);

        var resp = await httpClient.SendAsync(req, cancellationToken);
        if (!resp.IsSuccessStatusCode)
            return $"Fare rules request failed: {resp.StatusCode}";

        return await resp.Content.ReadAsStringAsync(cancellationToken);
    }
}
