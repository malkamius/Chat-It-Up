using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Channels;

namespace ChatItUp.Services
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
            
        }

        [HttpGet("GetRecentMessages")]
        public async Task<IActionResult> GetRecentMessages(Guid channelId, int skip = 0)
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
            var messages = await _chatService.GetRecentMessagesAsync(userId, channelId, skip);
            return Ok(messages);
        }

        [HttpGet("GetServers")]
        public async Task<IActionResult> GetServers()
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
            var messages = await _chatService.GetServers(userId);
            return Ok(messages);
        }

        [HttpGet("GetChannels")]
        public async Task<IActionResult> GetChannels(Guid serverId)
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
            var messages = await _chatService.GetChannels(userId, serverId);
            return Ok(messages);
        }
    }
}
