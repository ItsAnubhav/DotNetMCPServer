using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API.Models;

namespace API.Services
{
    public class QuadLabsAuthService : IQuadLabsAuthService
    {
        private readonly HttpClient _http;

        public QuadLabsAuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<QuadLabsAuthResponse?> GenerateLoginTokenAsync(QuadLabsAuthRequest request)
        {
            var resp = await _http.PostAsJsonAsync("/XChangeauth/api/auth/jwt/generateLoginToken", request);
            if (!resp.IsSuccessStatusCode)
            {
                return new QuadLabsAuthResponse
                {
                    Success = false,
                    Message = $"Upstream returned {(int)resp.StatusCode}"
                };
            }

            var data = await resp.Content.ReadFromJsonAsync<QuadLabsAuthResponse>();
            return data;
        }
    }
}
