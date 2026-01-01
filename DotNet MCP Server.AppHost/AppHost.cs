var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.API>("dotnet-mcp-server");
builder.AddMcpServer<Projects.DotNet_MCP_Server>("MCP Server");

builder.Build().Run();