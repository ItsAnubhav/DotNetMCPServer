using System;
using System.Collections.Generic;

namespace API.Models
{
    public enum ChatSessionStatus
    {
        Active,
        Closed
    }

    public class ChatSession
    {
        public Guid ChatSessionId { get; set; }
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public ChatSessionStatus Status { get; set; }

        public ICollection<ChatMessage>? Messages { get; set; }
        public ChatSummary? Summary { get; set; }
    }
}
