using System.Text;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;
using OpenAI.Chat;
using SdkChatMessage = OpenAI.Chat.ChatMessage;
using DbChatMessage = API.Models.ChatMessage;
using System.Runtime.CompilerServices;

namespace API.Services
{
    public interface IAgentService
    {
        Task<string> GetChatResponseAsync(string message, Guid chatSessionId, CancellationToken cancellationToken = default);
    }

    public class AgentService : IAgentService
    {
        private readonly AppDbContext _db;
        private readonly OpenAIClient _openAIClient;
        private readonly ILogger<AgentService> _logger;
        private readonly string _openAiModel;

        public AgentService(AppDbContext db, OpenAIClient openAiClient, IConfiguration config, ILogger<AgentService> logger)
        {
            _db = db;
            _openAIClient = openAiClient;
            _logger = logger;
            _openAiModel = config["OpenAI:Model"] ?? "gpt-4o";
        }

        public async Task<string> GetChatResponseAsync(string message, Guid chatSessionId, CancellationToken cancellationToken = default)
        {
            // Persist user message
            var userMsg = new DbChatMessage
            {
                MessageId = Guid.NewGuid(),
                ChatSessionId = chatSessionId,
                Role = ChatRole.User,
                Content = message,
                CreatedAt = DateTime.UtcNow
            };

            _db.ChatMessages.Add(userMsg);
            var session = await _db.ChatSessions.FindAsync(new object[] { chatSessionId }, cancellationToken);
            if (session != null)
            {
                session.LastMessageAt = userMsg.CreatedAt;
            }
            await _db.SaveChangesAsync(cancellationToken);

            // Build messages for SDK
            var messages = new List<SdkChatMessage>
            {
                new SystemChatMessage("You are a helpful assistant.")
            };

            var history = await _db.ChatMessages
                .Where(m => m.ChatSessionId == chatSessionId)
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
                    default:
                        messages.Add(new UserChatMessage(h.Content ?? string.Empty));
                        break;
                }
            }

            messages.Add(new UserChatMessage(message));

            try
            {
                var chatClient = _openAiClient.GetChatClient(_openAiModel);
                var response = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
                var assistantText = response.Value.Content.FirstOrDefault()?.Text ?? string.Empty;

                var assistantMsg = new DbChatMessage
                {
                    MessageId = Guid.NewGuid(),
                    ChatSessionId = chatSessionId,
                    Role = ChatRole.Assistant,
                    Content = assistantText,
                    CreatedAt = DateTime.UtcNow
                };

                _db.ChatMessages.Add(assistantMsg);
                if (session != null)
                {
                    session.LastMessageAt = assistantMsg.CreatedAt;
                }
                await _db.SaveChangesAsync(cancellationToken);

                return assistantText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenAI SDK");
                return string.Empty;
            }
        }

        public async IAsyncEnumerable<string> StreamChatAsync(string message, string conversationId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {

            // 1. Create a transport (stdio example for a local Node.js server)
            var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
            {
                Name = "MyTools",
                Command = "npx",
                Arguments = ["-y", "@modelcontextprotocol/server-github"]
            });

            // 2. Instantiate and connect the client
            var client = await McpClient.CreateAsync(clientTransport);

            // 3. List tools (McpClientTool inherits from AIFunction)
            IList<McpClientTool> tools = await client.ListToolsAsync();

            // 1. Set up the OpenAI client
            //IChatClient chatClient = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY")!)
            //    .AsChatClient("gpt-5-mini");

            

            // Save user message to database
            var userMessage = new DbChatMessage
            {
                ConversationId = conversationId,
                Role = "user",
                Content = message,
                Timestamp = DateTime.UtcNow
            };
            await _chatHistoryService.SaveMessageAsync(userMessage);

            // Get conversation history
            var history = await _chatHistoryService.GetMessagesAsync(conversationId, 20);
            var messages = BuildChatMessages(history, message);

            var chatClient = _openAIClient.GetChatClient("gpt-5-nano");

            // Get available tools for function calling
            //var tools = _toolRegistry.GetAllTools().Select(CreateChatTool).ToList();

            var options = new ChatCompletionOptions();
            foreach (var tool in tools)
            {
                Console.WriteLine($"Tool registered to agent: ${tool.FunctionName}");
                options.Tools.Add(tool);
            }

            var responseBuilder = new StringBuilder();
            var assistantMessage = new DbChatMessage
            {
                ConversationId = conversationId,
                Role = "assistant",
                Content = "",
                Timestamp = DateTime.UtcNow
            };

            await foreach (var update in chatClient.CompleteChatStreamingAsync(messages, options, cancellationToken))
            {
                // Stream content updates
                foreach (var contentPart in update.ContentUpdate)
                {
                    if (!string.IsNullOrEmpty(contentPart.Text))
                    {
                        responseBuilder.Append(contentPart.Text);
                        yield return contentPart.Text;
                    }
                }

                // Process completed function calls
                if (update.FinishReason == OpenAI.Chat.ChatFinishReason.ToolCalls)
                {
                    assistantMessage.Content = responseBuilder.ToString();
                    var savedAssistantMessage = await _chatHistoryService.SaveMessageAsync(assistantMessage);

                    var result = await ProcessToolCallsAsync(messages, options, conversationId, savedAssistantMessage.Id, cancellationToken);
                    foreach (var chunk in result)
                    {
                        yield return chunk;
                    }

                    // Don't save the message again at the end since we saved it here
                    responseBuilder.Clear();
                }
            }

            // Save final assistant message if no tool calls
            if (responseBuilder.Length > 0)
            {
                assistantMessage.Content = responseBuilder.ToString();
                await _chatHistoryService.SaveMessageAsync(assistantMessage);
            }
        }
    }
}