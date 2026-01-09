using System.Text.Json.Serialization;

namespace API.Models
{
    public class QuadLabsAuthResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public QuadLabsAuthData? Data { get; set; }
    }

    public class QuadLabsAuthData
    {
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("accessTokenExpiresIn")]
        public string? AccessTokenExpiresIn { get; set; }

        [JsonPropertyName("refreshTokenExpiresIn")]
        public string? RefreshTokenExpiresIn { get; set; }
    }
}
