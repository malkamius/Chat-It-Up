using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Tls;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Security.Claims;

namespace ChatItUp.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerImageController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ServerImageController(ChatService chatService)
        {
            _chatService = chatService;

        }

        [HttpGet("GetServerImage")]
        public async Task<IActionResult> GetServerImage(Guid serverId)
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

            var image = await _chatService.GetServerImageAsync(userId, serverId);
            var stream = new MemoryStream(image);
            {
                //image.Save(stream, ImageFormat.Png);
                return File(stream, "image/png");
            }
                
        }
    }
}
