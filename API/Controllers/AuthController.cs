using System.Threading.Tasks;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IQuadLabsAuthService _auth;

        public AuthController(IQuadLabsAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("generateLoginToken")]
        public async Task<IActionResult> GenerateLoginToken([FromBody] QuadLabsAuthRequest request)
        {
            var resp = await _auth.GenerateLoginTokenAsync(request);
            if (resp == null)
                return StatusCode(502, new { success = false, message = "Bad gateway" });

            return Ok(resp);
        }
    }
}
