using System.Text.Json.Serialization;

namespace McpServerApp.Services.Travog.Request
{
    public class QLAuthRequest
    {
        [JsonPropertyName("companyId")]
        public string CompanyId { get; set; } = string.Empty;

        [JsonPropertyName("accountNo")]
        public string AccountNo { get; set; } = string.Empty;

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;
    }
}
