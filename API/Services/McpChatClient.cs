using System.Text;
using ModelContextProtocol.Client;
using OpenAI;
using OpenAI.Chat;
using SdkChatMessage = OpenAI.Chat.ChatMessage;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class McpChatClient : IAsyncDisposable
{
    private McpClient? _mcpClient;
    private StdioClientTransport? _transport;
    private readonly OpenAIClient _openAiClient;
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;

    public McpChatClient(OpenAIClient openAiClient, IConfiguration config, AppDbContext db)
    {
        _openAiClient = openAiClient;
        _config = config;
        _db = db;
    }

    /// <summary>
    /// Connects to the "DotNet MCP Server" by launching the project with `dotnet run`.
    /// The path to the server project defaults to the workspace relative DotNet MCP Server csproj,
    /// but can be overridden via configuration `MCP:ServerProjectPath`.
    /// </summary>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (_mcpClient != null) return;

        var projectPath = _config["MCP:ServerProjectPath"] ?? Path.Combine("..", "DotNet MCP Server", "DotNet MCP Server.csproj");

        var options = new StdioClientTransportOptions
        {
            Name = "DotNetMcpClient",
            Command = "dotnet",
            Arguments = new[] { "run", "--project", projectPath }
        };

        _transport = new StdioClientTransport(options);
        _mcpClient = await McpClient.CreateAsync(_transport, cancellationToken: cancellationToken);
    }

    public async Task<IList<McpClientTool>> ListToolsAsync(CancellationToken cancellationToken = default)
    {
        if (_mcpClient == null) await ConnectAsync(cancellationToken);
        return await _mcpClient!.ListToolsAsync();
    }

    /// <summary>
    /// Process a single query using OpenAI Chat (ChatGPT) and the tools available from the MCP server.
    /// This mirrors the Python example from the MCP docs but uses OpenAI instead of Anthropic.
    /// </summary>
    public async Task<string> ProcessQueryAsync(string query, Guid? chatSessionId = null, CancellationToken cancellationToken = default)
    {
        if (_mcpClient == null) await ConnectAsync(cancellationToken);

        var tools = await _mcpClient!.ListToolsAsync();

        var messages = new List<SdkChatMessage>
        {
            new SystemChatMessage("You are a helpful assistant.")
        };

        // Load last 20 messages from DB if a chat session was provided
        if (chatSessionId.HasValue)
        {
            var history = await _db.ChatMessages
                .Where(m => m.ChatSessionId == chatSessionId.Value)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);

            foreach (var h in history.TakeLast(20))
            {
                switch (h.Role)
                {
                    case ChatRole.User:
                        messages.Add(new UserChatMessage(h.Content ?? string.Empty));
                        break;
                    case ChatRole.Assistant:
                        messages.Add(new AssistantChatMessage(h.Content ?? string.Empty));
                        break;
                    case ChatRole.System:
                        messages.Add(new SystemChatMessage(h.Content ?? string.Empty));
                        break;
                    case ChatRole.Tool:
                        // Tools can be represented as assistant messages for context
                        messages.Add(new AssistantChatMessage(h.Content ?? string.Empty));
                        break;
                    default:
                        messages.Add(new UserChatMessage(h.Content ?? string.Empty));
                        break;
                }
            }
        }

        // Add the new user message
        messages.Add(new UserChatMessage(query));

        // Persist user message if session provided
        if (chatSessionId.HasValue)
        {
            var userMsg = new API.Models.ChatMessage
            {
                MessageId = Guid.NewGuid(),
                ChatSessionId = chatSessionId.Value,
                Role = ChatRole.User,
                Content = query,
                CreatedAt = DateTime.UtcNow
            };
            _db.ChatMessages.Add(userMsg);
            var session = await _db.ChatSessions.FindAsync(new object[] { chatSessionId.Value }, cancellationToken);
            if (session != null)
            {
                session.LastMessageAt = userMsg.CreatedAt;
            }
            await _db.SaveChangesAsync(cancellationToken);
        }

        var model = _config["OpenAI:Model"] ?? "gpt-4o";
        var chatClient = _openAiClient.GetChatClient(model);

        var options = new ChatCompletionOptions();
        foreach (var t in tools)
        {
            var chatTool = ChatTool.CreateFunctionTool(
                functionName: t.Name,
                functionDescription: t.Description,
                functionParameters: BinaryData.FromString(t.JsonSchema.ToString())
            );
            options.Tools.Add(chatTool);
        }

        var response = await chatClient.CompleteChatAsync(messages, options, cancellationToken: cancellationToken);
        var assistantText = response.Value.Content.FirstOrDefault()?.Text ?? string.Empty;

        // Persist assistant message
        if (chatSessionId.HasValue)
        {
            var assistantMsg = new API.Models.ChatMessage
            {
                MessageId = Guid.NewGuid(),
                ChatSessionId = chatSessionId.Value,
                Role = ChatRole.Assistant,
                Content = assistantText,
                CreatedAt = DateTime.UtcNow
            };
            _db.ChatMessages.Add(assistantMsg);
            var session = await _db.ChatSessions.FindAsync(new object[] { chatSessionId.Value }, cancellationToken);
            if (session != null)
            {
                session.LastMessageAt = assistantMsg.CreatedAt;
            }
            await _db.SaveChangesAsync(cancellationToken);
        }

        return assistantText;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_mcpClient != null)
            {
                await _mcpClient.DisposeAsync();
                _mcpClient = null;
            }

            if (_transport != null)
            {
                _transport = null;
            }
        }
        catch
        {
            // swallow dispose errors
        }
    }
}
