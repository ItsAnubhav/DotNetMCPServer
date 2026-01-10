using Microsoft.AspNetCore.SignalR;
using API.Models;
using API.Services;
using API.Services.BookingDetails;

namespace API.Hubs;

public class ChatHub : Hub
{
    private readonly AgentService _agentService;
    private readonly IChatHistoryService _chatHistoryService;
    private readonly ILogger<ChatHub> _logger;
    private readonly IBookingDetailsService _bookingDetailsService;

    public ChatHub(AgentService agentService, IChatHistoryService chatHistoryService, ILogger<ChatHub> logger, IBookingDetailsService bookingDetailService)
    {
        _agentService = agentService;
        _chatHistoryService = chatHistoryService;
        _logger = logger;
        _bookingDetailsService = bookingDetailService;
    }

    public async Task SendMessage(string message, string? conversationId = null)
    {
        try
        {
            _logger.LogInformation("Received message from {ConnectionId}: {Message}", Context.ConnectionId, message);
            _logger.LogInformation("Starting message processing...");

            // Create or get conversation
            if (string.IsNullOrEmpty(conversationId))
            {
                _logger.LogInformation("Creating new conversation for session {SessionId}", Context.ConnectionId);
                var conversation = await _chatHistoryService.CreateConversationAsync(Context.ConnectionId);
                conversationId = conversation.Id;
                _logger.LogInformation("Created conversation with ID: {ConversationId}", conversationId);
                await Clients.Caller.SendAsync("ConversationCreated", conversationId);
            }

           
             


            await Clients.Caller.SendAsync("ReceiveMessage", "assistant", "");

            await foreach (var chunk in _agentService.StreamChatAsync(message, conversationId, Context.ConnectionAborted))
            {
                await Clients.Caller.SendAsync("ReceiveChunk", chunk);
            }

            // Signal completion
            await Clients.Caller.SendAsync("MessageComplete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from {ConnectionId}", Context.ConnectionId);
            await Clients.Caller.SendAsync("ReceiveError", "Sorry, I encountered an error processing your request.");
        }
    }

    public async Task GetConversationHistory(string conversationId)
    {
        try
        {
            var messages = await _chatHistoryService.GetMessagesAsync(conversationId);
            await Clients.Caller.SendAsync("ConversationHistory", messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversation history for {ConversationId}", conversationId);
            await Clients.Caller.SendAsync("ReceiveError", "Sorry, I couldn't retrieve the conversation history.");
        }
    }

    //public async Task GetConversations()
    //{
    //    try
    //    {
    //        var conversations = await _chatHistoryService.GetConversationsBySessionAsync(Context.ConnectionId);
    //        await Clients.Caller.SendAsync("ConversationsList", conversations);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error getting conversations for {ConnectionId}", Context.ConnectionId);
    //        await Clients.Caller.SendAsync("ReceiveError", "Sorry, I couldn't retrieve your conversations.");
    //    }
    //}


    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("✅ Client connected: {ConnectionId}", Context.ConnectionId);

        string? conversationId = Context.GetHttpContext()?.Request.Query["conversationId"];

        if (!string.IsNullOrEmpty(conversationId))
        {
            _logger.LogInformation("ℹ️ Client provided conversationId {ConversationId}", conversationId);
            await Clients.Caller.SendAsync("ConversationCreated", conversationId);

            // 🧠 Fetch all non-system messages
            var messages = await _chatHistoryService.GetNewMessagesAsync(conversationId);
            var visibleMessages = messages.Where(m => m.Role != "system").ToList();

            // 📨 Always send welcome message first
            var combinedMessages = new List<ChatMessage>
        {
            new ChatMessage
            {
                Role = "assistant",
                Content = "Welcome to TravelAgent! How can I help you plan your trip today?",
                //Timestamp = DateTime.UtcNow

            }
        };

            // Append existing conversation messages
            combinedMessages.AddRange(visibleMessages);

            // ✅ Send to client in correct order (welcome first)
            await Clients.Caller.SendAsync("ConversationHistory", combinedMessages);

            _logger.LogInformation("📜 Sent welcome + {Count} chat messages", visibleMessages.Count);
        }
        else
        {
            // 🆕 New conversation
            var conversation = await _chatHistoryService.CreateConversationAsync(Context.ConnectionId);
            conversationId = conversation.Id;

            _logger.LogInformation("🆕 Created new conversation {ConversationId}", conversationId);
            await Clients.Caller.SendAsync("ConversationCreated", conversationId);

            // 💬 Send only welcome message (no history)
            await Clients.Caller.SendAsync("ConversationHistory", new[]
            {
            new ChatMessage
            {
                Role = "assistant",
                Content = "Welcome to TravelAgent! How can I help you plan your trip today?",
                //Timestamp = DateTime.UtcNow
            }
        });
        }

        await base.OnConnectedAsync();
    }
     
    //public async Task ResumeConversation(string conversationId)
    //{
    //    try
    //    {
    //        _logger.LogInformation("🔁 Resuming conversation {ConversationId} for {ConnectionId}", conversationId, Context.ConnectionId);

    //        var messages = await _chatHistoryService.GetMessagesAsync(conversationId);

    //        // Filter out system messages
    //        var visibleMessages = messages.Where(m => m.Role != "system").ToList();

    //        if (visibleMessages.Any())
    //        {
    //            await Clients.Caller.SendAsync("ConversationHistory", visibleMessages);
    //            _logger.LogInformation("📜 Sent filtered conversation history ({Count} messages)", visibleMessages.Count);
    //        }
    //        else
    //        {
    //            await Clients.Caller.SendAsync("ConversationEmpty", conversationId);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "❌ Error resuming conversation {ConversationId}", conversationId);
    //        await Clients.Caller.SendAsync("ReceiveError", "Could not resume previous conversation.");
    //    }
    //}





    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
     
    public async Task RegisterPageInfo(
    string token,
    string? page,
    string? bookingRef,
    string? source,
    string? companyId,
    string? conversationId)
    {
        try
        {
            _logger.LogInformation("📥 Received initial info - Token: {Token}, Page: {Page}, BookingRef: {BookingRef}, ConversationId: {ConversationId}",
                token, page, bookingRef, conversationId);

            string responseMessage = string.Empty;
            bool isBookingDetail = false;

            // ✅ Build message content dynamically
            if (!string.IsNullOrEmpty(bookingRef))
            {
                var bookingDetails = await _bookingDetailsService.GetBookingDetailsAsync(token, page, bookingRef, source, companyId);

                responseMessage =
                    $"📄 Here are your booking details. Please note that you don’t have permission to make any changes to this information or add or modify traveller details.:\n{bookingDetails}";
                isBookingDetail = true;
            }
            else if (!string.IsNullOrEmpty(page))
            {
                responseMessage = $"You're currently on the **{page}** page. How can I assist you here?";
            }

            // ✅ Save only if we have valid response + conversationId
            if (!string.IsNullOrEmpty(responseMessage) && !string.IsNullOrEmpty(conversationId))
            {
                // ✅ Prevent duplicate booking detail messages
                if (isBookingDetail)
                {
                    bool alreadyExists = await _chatHistoryService.MessageExistsAsync(conversationId, true);
                    if (alreadyExists)
                    {
                        _logger.LogInformation("⚙️ Skipped duplicate booking detail message for {ConversationId}", conversationId);
                        return;
                    }
                }

                // 💾 Save system message
                var systemMessage = new ChatMessage
                {
                    ConversationId = conversationId,
                    Role = "system",
                    Content = responseMessage,
                    Timestamp = DateTime.UtcNow,
                    IsBookingDetail = isBookingDetail // ✅ added flag
                };

                await _chatHistoryService.SaveMessageAsync(systemMessage);
                _logger.LogInformation("💾 Saved system message for conversation {ConversationId}", conversationId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Error in RegisterPageInfo for conversation {ConversationId}", conversationId);
            await Clients.Caller.SendAsync("ReceiveError", "Unable to process page info. Please try again later.");
        }
    }





}