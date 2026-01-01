var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DotNet_MCP_Server>("dotnet-mcp-server");

builder.Build().Run();