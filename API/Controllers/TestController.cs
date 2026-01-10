using API.Models;
using McpServerApp.Services.Travog;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly ITravogAPIService service;


        public TestController(ITravogAPIService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Abc()
        {
            var result = await service.GetBookingDetailsAsync("669463");
            return Ok("TestController is working!");
        }

        //[HttpPost("generateLoginToken")]
        //public async Task<IActionResult> GenerateLoginToken([FromBody] QuadLabsAuthRequest request)
        //{
        //    var resp = await _auth.GenerateLoginTokenAsync(request);
        //    if (resp == null)
        //        return StatusCode(502, new { success = false, message = "Bad gateway" });

        //    return Ok(resp);
        //}
    }
}
