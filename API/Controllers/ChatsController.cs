using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ChatsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("sessions/{chatSessionId}/messages")]
        public async Task<IActionResult> GetMessagesBySession(Guid chatSessionId)
        {
            var messages = await _db.ChatMessages
                .Where(m => m.ChatSessionId == chatSessionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet("messages/by-user/{userId}")]
        public async Task<IActionResult> GetMessagesByUser(string userId)
        {
            var messages = await _db.ChatMessages
                .Include(m => m.ChatSession)
                .Where(m => m.ChatSession != null && m.ChatSession.UserId == userId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return Ok(messages);
        }
    }
}
