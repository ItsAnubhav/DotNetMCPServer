using API.Data;
using API.Hubs;
using API.Services;
using McpServerApp.Services.Travog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenAI;
using System.Net.Http.Headers;
//using API.Hubs;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Register OpenAI HttpClient (uses config OpenAI:ApiKey and optional OpenAI:BaseUrl/OpenAI:Model)
builder.Services.AddHttpClient("OpenAI", client =>
{
    var baseUrl = builder.Configuration["OpenAI:BaseUrl"] ?? "https://api.openai.com/";
    client.BaseAddress = new Uri(baseUrl);
    var apiKey = builder.Configuration["OpenAI:ApiKey"];
    if (!string.IsNullOrEmpty(apiKey))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<IAgentService, AgentService>();

// Register OpenAI SDK client for server-side usage (uses OpenAI:ApiKey)
builder.Services.AddSingleton(sp =>
{
    var apiKey = builder.Configuration["OpenAI:ApiKey"];
    if (string.IsNullOrEmpty(apiKey))
        throw new InvalidOperationException("OpenAI:ApiKey is not configured");

    return new OpenAIClient("");
});

// Register MCP client that connects to the DotNet MCP Server and uses OpenAI (ChatGPT)
builder.Services.AddScoped<McpChatClient>();


// Configure EF Core SQL Server
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(conn))
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conn));
}

builder.Services.AddScoped<ITravogAPIService, TravogAPIService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    //app.UseSwagger();
    //app.UseSwaggerUI(c =>
    //{
    //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    //    c.RoutePrefix = string.Empty; // serve swagger UI at application root
    //});
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Map attribute-routed controllers (e.g., [ApiController], [Route("api/[controller]")])
app.MapControllers();

// SignalR hubs
app.MapHub<ChatHub>("/chatHub");


app.Run();
