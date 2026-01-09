
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace McpServerApp.Services.Travog;

public class TravogAPIService
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
	public async Task<string?> GetBookingDetailsAsync(string bookingRef, McpServerApp.Services.Travog.Request.QLAuthRequest authRequest, string? scopeToken = null, CancellationToken cancellationToken = default)
	{
		var url = "https://preprod.quadlabs.net/XchangeServices/api/XchangeBooking/getBookingDetails";

		// Acquire access token using the injected QL auth service
		var accessToken = await _qlAuthService.GetTokenAsync(authRequest, cancellationToken);
		if (string.IsNullOrEmpty(accessToken))
		{
			_logger.LogWarning("GetBookingDetailsAsync: failed to obtain access token for bookingRef={BookingRef}", bookingRef);
			return null;
		}

		var req = new HttpRequestMessage(HttpMethod.Post, url);
		req.Headers.Accept.ParseAdd("*/*");
		req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
		if (!string.IsNullOrEmpty(scopeToken))
			req.Headers.Add("X-Scope-Token", scopeToken);

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

		var content = await resp.Content.ReadAsStringAsync(cancellationToken);
		return content;
	}
}