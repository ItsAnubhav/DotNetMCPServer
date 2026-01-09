using System.Threading.Tasks;
using API.Models;

namespace API.Services
{
    public interface IQuadLabsAuthService
    {
        Task<QuadLabsAuthResponse?> GenerateLoginTokenAsync(QuadLabsAuthRequest request);
    }
}
