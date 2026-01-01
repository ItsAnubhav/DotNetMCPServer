using System;

namespace API.Models
{
    public enum ChatRole
    {
        User,
        Assistant,
        Tool,
        System
    }

    public class ChatMessage
    {
        public Guid MessageId { get; set; }
        public Guid ChatSessionId { get; set; }
        public ChatRole Role { get; set; }
        public string? Content { get; set; }
        public string? ToolName { get; set; }
        public int TokenCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public ChatSession? ChatSession { get; set; }
    }
}
