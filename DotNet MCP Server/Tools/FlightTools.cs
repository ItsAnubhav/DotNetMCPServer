using ModelContextProtocol.Server;
using System.ComponentModel;
using Microsoft.Extensions.Options;
using McpServerApp.Services;
using McpServerApp.Services.Saudia;
using Microsoft.Extensions.Logging;

namespace McpServerApp.Tools;

[McpServerToolType]
public static class FlightTools
{
    [McpServerTool]
    [Description("Search available flights between origin and destination on a date.")]
    public static async Task<string> FlightSearch(
        HttpClient httpClient,
        IAuthService authService,
        ILoggerFactory loggerFactory,
        [Description("IATA origin code")] string origin,
        [Description("IATA destination code")] string destination,
        [Description("Date in YYYY-MM-DD")] string date,
        CancellationToken cancellationToken = default)
    {
        var logger = loggerFactory.CreateLogger("FlightTools");
        logger.LogInformation("FlightSearch called for {Origin}->{Destination} on {Date}", origin, destination, date);
        return "Flight search tool is not implemented yet.";
        // var options = opts.Value;
        // if (string.IsNullOrEmpty(options.FlightSearchBaseUrl))
        //     return "Flight search base URL not configured.";

        // var url = $"{options.FlightSearchBaseUrl.TrimEnd('/')}/search?origin={origin}&destination={destination}&date={date}";
        // var req = new HttpRequestMessage(HttpMethod.Get, url);
        // var token = await authService.GetTokenAsync(cancellationToken);
        // if (!string.IsNullOrEmpty(token)) req.Headers.Authorization = new("Bearer", token);

        // var resp = await httpClient.SendAsync(req, cancellationToken);
        // if (!resp.IsSuccessStatusCode)
        //     return $"Flight search request failed: {resp.StatusCode}";

        // var body = await resp.Content.ReadAsStringAsync(cancellationToken);
        // return body;
    }

    [McpServerTool]
    [Description("Get booking details by booking id.")]
    public static async Task<string> BookingDetails(
        HttpClient httpClient,
        ISaudiaService saudiaService,
        ILoggerFactory loggerFactory,
        [Description("Booking identifier")] string bookingId,
        [Description("Passenger last name")] string lastName,
        CancellationToken cancellationToken = default)
    {
        var logger = loggerFactory.CreateLogger("FlightTools");
        logger.LogInformation("BookingDetails: fetching booking {BookingId} for {LastName}", bookingId, lastName);
        var orderDetails = await saudiaService.GetOrderDetailsAsync(bookingId, lastName, cancellationToken);
        if (orderDetails is null)
        {
            logger.LogWarning("BookingDetails: failed to retrieve booking {BookingId}", bookingId);
            return "Failed to retrieve booking details.";
        }
        logger.LogDebug("BookingDetails: retrieved booking {BookingId}", bookingId);
        return System.Text.Json.JsonSerializer.Serialize(orderDetails);
        // var options = opts.Value;
        // if (string.IsNullOrEmpty(options.BookingDetailsBaseUrl))
        //     return "Booking details base URL not configured.";

        // var url = $"{options.BookingDetailsBaseUrl.TrimEnd('/')}/bookings/{bookingId}";
        // var req = new HttpRequestMessage(HttpMethod.Get, url);
        // var token = await authService.GetTokenAsync(cancellationToken);
        // if (!string.IsNullOrEmpty(token)) req.Headers.Authorization = new("Bearer", token);

        // var resp = await httpClient.SendAsync(req, cancellationToken);
        // if (!resp.IsSuccessStatusCode)
        //     return $"Booking details request failed: {resp.StatusCode}";

        // return await resp.Content.ReadAsStringAsync(cancellationToken);
    }

    [McpServerTool]
    [Description("Fetch fare rules by fare id.")]
    public static async Task<string> FareRules(
        HttpClient httpClient,
        IAuthService authService,
        ILoggerFactory loggerFactory,
        [Description("Fare identifier")] string fareId,
        CancellationToken cancellationToken = default)
    {
        var logger = loggerFactory.CreateLogger("FlightTools");
        logger.LogInformation("FareRules called for {FareId}", fareId);
        return "Fare rules tool is not implemented yet.";
        // var options = opts.Value;
        // if (string.IsNullOrEmpty(options.FareRulesBaseUrl))
        //     return "Fare rules base URL not configured.";

        // var url = $"{options.FareRulesBaseUrl.TrimEnd('/')}/fares/{fareId}/rules";
        // var req = new HttpRequestMessage(HttpMethod.Get, url);
        // var token = await authService.GetTokenAsync(cancellationToken);
        // if (!string.IsNullOrEmpty(token)) req.Headers.Authorization = new("Bearer", token);

        // var resp = await httpClient.SendAsync(req, cancellationToken);
        // if (!resp.IsSuccessStatusCode)
        //     return $"Fare rules request failed: {resp.StatusCode}";

        // return await resp.Content.ReadAsStringAsync(cancellationToken);
    }
}
