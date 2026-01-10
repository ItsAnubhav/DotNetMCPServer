using ModelContextProtocol.Server;
using System.ComponentModel;
using Microsoft.Extensions.Options;
using McpServerApp.Services;
using McpServerApp.Services.Saudia;
using Microsoft.Extensions.Logging;
using McpServerApp.Services.Travog;
using DotNet_MCP_Server.Tools.Models;
using ToonSharp;

namespace McpServerApp.Tools;

[McpServerToolType]
public static class BookingTools
{

    [McpServerTool]
    [Description("Get booking details by booking id")]
    public static async Task<string> BookingDetails(
        HttpClient httpClient,
        ITravogAPIService service,
        ILoggerFactory loggerFactory,
        [Description("Booking identifier")] string bookingId,
        CancellationToken cancellationToken = default)
    {
        var logger = loggerFactory.CreateLogger("FlightTools");
        logger.LogInformation("BookingDetails: fetching booking {BookingId}", bookingId);
        var orderDetails = await service.GetBookingDetailsAsync(bookingId, cancellationToken: cancellationToken);
        if (orderDetails is null)
        {
            logger.LogWarning("BookingDetails: failed to retrieve booking {BookingId}", bookingId);
            return "Failed to retrieve booking details.";
        }
        logger.LogDebug("BookingDetails: retrieved booking {BookingId}", bookingId);

        // Serialize the booking details to Summary Object for output
        BookingSummary bookingSummary = BookingSummary.FromDetail(orderDetails);
        return ToonSerializer.Serialize(bookingSummary);
    }

    [McpServerTool]
    [Description("Fetch fare rules by flight id")]
    public static async Task<string> FareRules(
        HttpClient httpClient,
        ITravogAPIService service,
        ILoggerFactory loggerFactory,
        [Description("Flight id to fetch fare rules for")] string flightId,
        CancellationToken cancellationToken = default)
    {
        var logger = loggerFactory.CreateLogger("FlightTools");
        logger.LogInformation("FareRules called for {FlightId}", flightId);

        var fareRules = await service.GetFareRulesAsync(flightId, cancellationToken: cancellationToken);
        if (fareRules is null)
        {
            logger.LogWarning("FareRules: failed to retrieve fare rules for {FlightId}", flightId);
            return "Failed to retrieve fare rules.";
        }
        logger.LogDebug("FareRules: retrieved fare rules for {FlightId}", flightId);
        
        FareRulesSummary summary = FareRulesSummary.FromDetails(fareRules);

        return ToonSerializer.Serialize(summary);

        //return "Fare rules tool is not implemented yet.";
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