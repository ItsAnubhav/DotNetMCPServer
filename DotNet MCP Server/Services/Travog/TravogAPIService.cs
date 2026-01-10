
using DotNet_MCP_Server.Services.Travog.Response;
using McpServerApp.Services.Travog.Response;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace McpServerApp.Services.Travog;

public interface ITravogAPIService
{
    Task<BookingDetailResponse?> GetBookingDetailsAsync(string bookingRef, CancellationToken cancellationToken = default);
	Task<FareRulesDetail?> GetFareRulesAsync(string flightId, CancellationToken cancellationToken = default);
}

public class TravogAPIService : ITravogAPIService
{
	private readonly IHttpClientFactory _factory;
	private readonly ILogger<TravogAPIService> _logger;
	private readonly IQLAuthService _qlAuthService;

	public TravogAPIService(IHttpClientFactory factory, IQLAuthService qlAuthService, ILogger<TravogAPIService> logger)
	{
		_factory = factory;
		_qlAuthService = qlAuthService;
		_logger = logger;
	}

	/// <summary>
	/// Calls the QuadLabs booking details endpoint with provided tokens and booking reference.
	/// Mirrors the curl in the request: POST https://preprod.quadlabs.net/XchangeServices/api/XchangeBooking/getBookingDetails
	/// </summary>
	public async Task<BookingDetailResponse?> GetBookingDetailsAsync(string bookingRef, CancellationToken cancellationToken = default)
	{
		var url = "https://preprod.quadlabs.net/XchangeServices/api/XchangeBooking/getBookingDetails";

		// Acquire access token using the injected QL auth service
		// var accessToken = await _qlAuthService.GetTokenAsync(authRequest, cancellationToken);
        var accessToken = await _qlAuthService.GetDataTokenAsync("QLABS12345", "sa", "Qu@d1@bs", cancellationToken);
		if (string.IsNullOrEmpty(accessToken?.identityToken) || string.IsNullOrEmpty(accessToken?.scopeToken))
		{
            _logger.LogWarning("GetBookingDetailsAsync: identityToken or scopeToken is null or empty for bookingRef={BookingRef}", bookingRef);
			_logger.LogWarning("GetBookingDetailsAsync: failed to obtain access token for bookingRef={BookingRef}", bookingRef);
			return null;
		}

		var req = new HttpRequestMessage(HttpMethod.Post, url);
		req.Headers.Accept.ParseAdd("*/*");
		req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.identityToken);
		if (!string.IsNullOrEmpty(accessToken.scopeToken))
			req.Headers.Add("X-Scope-Token", accessToken.scopeToken);

		var payload = new { bookingRef };
		req.Content = JsonContent.Create(payload);
		req.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

		var client = _factory.CreateClient();
		HttpResponseMessage resp;
		try
		{
			resp = await client.SendAsync(req, cancellationToken);
		}
		catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			_logger.LogInformation("GetBookingDetailsAsync cancelled for bookingRef={BookingRef}", bookingRef);
			throw;
		}

		if (!resp.IsSuccessStatusCode)
		{
			_logger.LogWarning("GetBookingDetailsAsync: upstream returned {StatusCode} for bookingRef={BookingRef}", resp.StatusCode, bookingRef);
			return null;
		}

		//var content = await resp.Content.ReadAsStringAsync(cancellationToken);
		var content = await resp.Content.ReadFromJsonAsync<BookingDetailResponse>(cancellationToken: cancellationToken);
		return content;
	}

	public async Task<FareRulesDetail?> GetFareRulesAsync(string flightId, CancellationToken cancellationToken = default)
	{
		// curl --location 'https://preprod.quadlabs.net/XchangeServices/api/XchangeBooking/getFareRules' \
		// --header 'Content-Type: application/json' \
		// --header 'X-Scope-Token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1YzVhZGUxNC02YTE3LTRhOWMtOTA5Ny1lNTNmZGQ2OWQ4MmMiLCJsaW5rZWRfdG8iOiJjYjM3NDdkYy1mMjJiLTQzMWYtYWY5Yy00NmI3OTY2YjJjNmQiLCJzY29wZSI6ImFwaTEucmVhZCwgYXBpMS53cml0ZSIsIm5iZiI6MTc2ODA0MTY3MCwiZXhwIjoxNzY4MDQ1MjcwLCJpYXQiOjE3NjgwNDE2NzAsImlzcyI6IlF1YWRsYWJzWGNoYW5nZSIsImF1ZCI6IlF1YWRsYWJzWGNoYW5nZUNsaWVudCJ9.50UBSqSWkKZsclTtuzMwhnQFwHe85jxYoycucgtLYZU' \
		// --header 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJjYjM3NDdkYy1mMjJiLTQzMWYtYWY5Yy00NmI3OTY2YjJjNmQiLCJjb21wYW55SWQiOiJRTGFiczEyMzQ1IiwidXNlcm5hbWUiOiJzYSIsIm5iZiI6MTc2ODA0MTY3MCwiZXhwIjoxNzY4MDQ1MjcwLCJpYXQiOjE3NjgwNDE2NzAsImlzcyI6IlF1YWRsYWJzWGNoYW5nZSIsImF1ZCI6IlF1YWRsYWJzWGNoYW5nZUNsaWVudCJ9.TF2MyowxUHbPnRqTp0-5BBm5oNOEQGpV4XzSI6CRSkg' \
		// --data '{
		// "flightId": "588805"
		// }'
		var url = "https://preprod.quadlabs.net/XchangeServices/api/XchangeBooking/getFareRules";
		// Acquire access token using the injected QL auth service
		var accessToken = await _qlAuthService.GetDataTokenAsync("QLABS12345", "sa", "Qu@d1@bs", cancellationToken);
		if (string.IsNullOrEmpty(accessToken?.identityToken) || string.IsNullOrEmpty(accessToken?.scopeToken))
		{
			_logger.LogWarning("GetFareRulesAsync: identityToken or scopeToken is null or empty for flightId={FlightId}", flightId);
			_logger.LogWarning("GetFareRulesAsync: failed to obtain access token for flightId={FlightId}", flightId);
			return null;
		}
		var req = new HttpRequestMessage(HttpMethod.Post, url);
		req.Headers.Accept.ParseAdd("*/*");
		req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.identityToken);
		if (!string.IsNullOrEmpty(accessToken.scopeToken))
			req.Headers.Add("X-Scope-Token", accessToken.scopeToken);
		var payload = new { flightId };
		req.Content = JsonContent.Create(payload);
		req.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
		var client = _factory.CreateClient();
		HttpResponseMessage resp;
		try
		{
			resp = await client.SendAsync(req, cancellationToken);
		}
		catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			_logger.LogInformation("GetFareRulesAsync cancelled for flightId={FlightId}", flightId);
			throw;
		}
		if (!resp.IsSuccessStatusCode)
		{
			_logger.LogWarning("GetFareRulesAsync: upstream returned {StatusCode} for flightId={FlightId}", resp.StatusCode, flightId);
			return null;
		}
		//var content = await resp.Content.ReadAsStringAsync(cancellationToken);
		var content = await resp.Content.ReadFromJsonAsync<FareRulesDetail>(cancellationToken: cancellationToken);
        return content;
	}
}