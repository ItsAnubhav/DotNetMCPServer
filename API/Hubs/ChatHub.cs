using Microsoft.AspNetCore.SignalR;
using API.Services;
using System.Runtime.CompilerServices;

namespace API.Hubs;

public class ChatHub : Hub
{
    private readonly IAgentService _agentService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IAgentService agentService, ILogger<ChatHub> logger)
    {
        _agentService = agentService;
        _logger = logger;
    }

    /// <summary>
    /// Streams assistant responses token-by-token to the caller.
    /// </summary>
    public async IAsyncEnumerable<string> StreamChat(
        string message,
        string conversationId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting streaming chat for connection {ConnectionId}, conversation {ConversationId}",
            Context.ConnectionId,
            conversationId
        );

        // Link SignalR disconnects to agent cancellation
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            Context.ConnectionAborted
        );

        await foreach (var token in _agentService.StreamChatAsync(
            message,
            conversationId,
            linkedCts.Token))
        {
            yield return token;
        }

        _logger.LogInformation(
            "Streaming chat completed for connection {ConnectionId}",
            Context.ConnectionId
        );
    }
}
