using System;

namespace API.Models
{
    public class ChatMessageDto
    {
        public Guid MessageId { get; set; }
        public Guid ChatSessionId { get; set; }
        public ChatRole Role { get; set; }
        public string? Content { get; set; }
        public string? ToolName { get; set; }
        public int TokenCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
