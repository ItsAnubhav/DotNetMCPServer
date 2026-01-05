using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using McpServerApp.Helpers;
using McpServerApp.Services;
using McpServerApp.Services.Saudia;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly(); // Loads tools from the current assembly

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpHelper, HttpHelper>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<ISaudiaService, SaudiaService>();
// builder.Services.Configure<ExternalApisOptions>(builder.Configuration.GetSection("ExternalApis"));
// builder.Services.AddSingleton<IExternalAuthService, AuthService>();

var app = builder.Build();
app.Run();

[McpServerToolType]
public static class MyTools
{
    [McpServerTool, Description("Adds two numbers")]
    public static double Add(double a, double b)
    {
        return a + b;
    }
}

// Options and services
// public record ExternalApisOptions
// {
//     public bool UseExternalAuth { get; init; }
//     public string? FlightSearchBaseUrl { get; init; }
//     public string? BookingDetailsBaseUrl { get; init; }
//     public string? FareRulesBaseUrl { get; init; }
//     public string? AuthEndpoint { get; init; }
//     public string? AuthClientId { get; init; }
//     public string? AuthClientSecret { get; init; }
//     public string? AuthScope { get; init; }
//     public string? AuthFact { get; init; }
// }

// public interface IExternalAuthService
// {
//     Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
// }
// ExternalAuthService removed — use `AuthService` in Services/AuthService.cs instead.
