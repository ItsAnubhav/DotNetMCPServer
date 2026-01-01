var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.API>("mcp-apis");
builder.AddProject<Projects.DotNet_MCP_Server>("MCPServer");

builder.Build().Run();