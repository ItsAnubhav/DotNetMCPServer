using System.Text.Json.Serialization;

namespace API.Models
{
    public class QuadLabsAuthRequest
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
