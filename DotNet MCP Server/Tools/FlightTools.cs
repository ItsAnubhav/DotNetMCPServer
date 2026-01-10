using ModelContextProtocol.Server;
using System.ComponentModel;
using Microsoft.Extensions.Options;
using McpServerApp.Services;
using McpServerApp.Services.Saudia;
using Microsoft.Extensions.Logging;
using McpServerApp.Services.Travog;

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

}
