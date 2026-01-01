var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.API>("mcp-apis");
builder.AddProject<Projects.DotNet_MCP_Server>("MCPServer");
//npx @modelcontextprotocol/inspector dotnet run --project "D:\DotNet MCP Server\DotNet MCP Server\DotNet MCP Server.csproj
builder.Build().Run();