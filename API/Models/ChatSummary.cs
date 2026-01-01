using System;

namespace API.Models
{
    public class ChatSummary
    {
        public Guid ChatSessionId { get; set; }
        public string? SummaryText { get; set; }
        public DateTime? LastSummarizedAt { get; set; }

        public ChatSession? ChatSession { get; set; }
    }
}
