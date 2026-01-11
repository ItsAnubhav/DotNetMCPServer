using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using OpenAI;
using OpenAI.Chat;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using DbChatMessage = API.Models.ChatMessage;

namespace API.Services;

public interface IAgentService
{
    IAsyncEnumerable<string> StreamChatAsync(string message, string conversationId, [EnumeratorCancellation] CancellationToken cancellationToken = default);
}

public class AgentService : IAgentService
{
    private readonly AppDbContext _db;
    private readonly OpenAIClient _openAIClient;
    private readonly ILogger<AgentService> _logger;
    private readonly string _openAiModel;

    private readonly StdioClientTransport _mcpTransport;

    public AgentService(AppDbContext db, OpenAIClient openAiClient, IConfiguration config, ILogger<AgentService> logger)
    {
        _db = db;
        _openAIClient = openAiClient;
        _logger = logger;
        _openAiModel = config["OpenAI:Model"] ?? "gpt-4o";

        // Use the local DotNet MCP Server project for tool execution. The path is relative
        // to the API project directory. Adjust if your project layout differs.
        _mcpTransport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "DotNetMCPServer",
            Command = "dotnet",
            Arguments = new[] { "run", "--project", "../DotNet MCP Server/DotNet MCP Server.csproj", "--no-build" }
        });
    }

    public async IAsyncEnumerable<string> StreamChatAsync(
        string message,
        string conversationId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var mcpClient = await McpClient.CreateAsync(_mcpTransport);
        var mcpTools = await mcpClient.ListToolsAsync();

        if (!Guid.TryParse(conversationId, out var sessionGuid))
            throw new ArgumentException("Invalid conversationId");

        var history = await _db.ChatMessages
            .Where(m => m.ChatSessionId == sessionGuid)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        // Save user message
        var userDbMessage = new DbChatMessage
        {
            MessageId = Guid.NewGuid(),
            ChatSessionId = sessionGuid,
            Role = Models.ChatRole.User,
            Content = message,
            CreatedAt = DateTime.UtcNow
        };

        _db.ChatMessages.Add(userDbMessage);
        await _db.SaveChangesAsync(cancellationToken);

        // Build OpenAI message list
        var messages = new List<ChatMessage>();
        foreach (var msg in history)
        {
            messages.Add(msg.Role switch
            {
                Models.ChatRole.User => new UserChatMessage(msg.Content),
                Models.ChatRole.Assistant => new AssistantChatMessage(msg.Content),
                Models.ChatRole.Tool => new ToolChatMessage(msg.Content),
            });
        }

        messages.Add(new UserChatMessage(message));

        var chatClient = _openAIClient.GetChatClient(_openAiModel);

        var options = new ChatCompletionOptions();

        // Attach MCP tools to OpenAI
        foreach (var tool in mcpTools)
        {
            options.Tools.Add(ChatTool.CreateFunctionTool(
                tool.Name,
                tool.Description,
                BinaryData.FromString(tool.JsonSchema.GetRawText())
            ));
        }

        // Create assistant DB row early (for streaming)
        var assistantDbMessage = new DbChatMessage
        {
            MessageId = Guid.NewGuid(),
            ChatSessionId = sessionGuid,
            Role = Models.ChatRole.Assistant,
            Content = string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        _db.ChatMessages.Add(assistantDbMessage);
        await _db.SaveChangesAsync(cancellationToken);

        var assistantTextBuffer = new StringBuilder();

        // === Main agent loop ===
        while (true)
        {
            
            var toolNames = new Dictionary<string, string>(); // ToolCallId -> FunctionName
            var toolArgBuffers = new Dictionary<string, StringBuilder>(); // ToolCallId -> args

            await foreach (var update in chatClient.CompleteChatStreamingAsync(messages, options, cancellationToken))
            {
                // Stream assistant text
                if (update.ContentUpdate is not null)
                {
                    foreach (var part in update.ContentUpdate)
                    {
                        if (!string.IsNullOrEmpty(part.Text))
                        {
                            assistantTextBuffer.Append(part.Text);
                            assistantDbMessage.Content = assistantTextBuffer.ToString();
                            assistantDbMessage.CreatedAt = DateTime.UtcNow;

                            _db.ChatMessages.Update(assistantDbMessage);
                            await _db.SaveChangesAsync(cancellationToken);

                            yield return part.Text;
                        }
                    }
                }

                // Accumulate tool calls
                if (update.ToolCallUpdates is not null)
                {
                    foreach (var call in update.ToolCallUpdates)
                    {
                        // Capture tool name once
                        if (!toolNames.ContainsKey(call.ToolCallId))
                        {
                            toolNames[call.ToolCallId] = call.FunctionName;
                        }

                        // Accumulate arguments
                        if (!toolArgBuffers.TryGetValue(call.ToolCallId, out var buffer))
                        {
                            buffer = new StringBuilder();
                            toolArgBuffers[call.ToolCallId] = buffer;
                        }

                        if (call.FunctionArgumentsUpdate is not null)
                        {
                            buffer.Append(call.FunctionArgumentsUpdate);
                        }

                    }
                }

                if (update.FinishReason == ChatFinishReason.ToolCalls)
                    break;

                if (update.FinishReason == ChatFinishReason.Stop)
                {
                    assistantDbMessage.Content = assistantTextBuffer.ToString();
                    assistantDbMessage.CreatedAt = DateTime.UtcNow;
                    _db.ChatMessages.Update(assistantDbMessage);
                    await _db.SaveChangesAsync(cancellationToken);
                    yield break;
                }
            }

            foreach (var toolCallId in toolNames.Keys)
            {
                var functionName = toolNames[toolCallId];
                var argsJson = toolArgBuffers[toolCallId].ToString();

                _logger.LogInformation(
                    "Executing MCP tool {Tool} with args {Args}",
                    functionName,
                    argsJson
                );

                IReadOnlyDictionary<string, object?>? arguments = null;

                if (!string.IsNullOrWhiteSpace(argsJson))
                {
                    arguments = JsonSerializer.Deserialize<Dictionary<string, object?>>(
                        argsJson,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    );
                }

                var result = await mcpClient.CallToolAsync(
                    functionName,
                    arguments,
                    cancellationToken: cancellationToken
                );

                // Save tool output to DB
                _db.ChatMessages.Add(new DbChatMessage
                {
                    MessageId = Guid.NewGuid(),
                    ChatSessionId = sessionGuid,
                    Role = Models.ChatRole.Tool,
                    Content = FlattenToolResult(result),
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync(cancellationToken);

                // Send tool result back to OpenAI
                messages.Add(new ToolChatMessage(
                    toolCallId,
                    FlattenToolResult(result)
                ));
            }

            // Allow model to continue reasoning after tools
            messages.Add(new AssistantChatMessage(assistantTextBuffer.ToString()));
        }
    }

    private static string FlattenToolResult(CallToolResult result)
    {
        if (result?.Content == null || result.Content.Count == 0)
            return string.Empty;

        var sb = new StringBuilder();

        foreach (var block in result.Content)
        {
            switch (block)
            {
                case TextContentBlock textBlock:
                    sb.Append(textBlock.Text);
                    break;

                case ImageContentBlock imageBlock:
                    // Optional: describe images for the model
                    sb.Append("[Image]");
                    break;

                case EmbeddedResourceBlock resourceBlock:
                    // Optional: serialize structured resources
                    sb.Append(JsonSerializer.Serialize(resourceBlock));
                    break;

                default:
                    // Fallback for future MCP block types
                    sb.Append(JsonSerializer.Serialize(block));
                    break;
            }
        }

        return sb.ToString();
    }
}