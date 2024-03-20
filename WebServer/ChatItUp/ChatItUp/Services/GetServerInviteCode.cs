using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace ChatItUp.Services
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GetServerInviteCode : ControllerBase
    {
        private Context.CIUDataDbContext _context;

        public GetServerInviteCode(Context.CIUDataDbContext context) 
        {
            _context = context;
        }

        [HttpGet("GetInvite")]
        public async Task<IActionResult> GetInvite(Guid serverId)
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
            var server = _context.Servers.FirstOrDefault(s => s.Id == serverId);
            var invite = _context.ServerInviteCodes.FirstOrDefault(i => i.ServerId == serverId);

            if (server == null)
            {
                return BadRequest("Server not found");
            }
            else if (invite == null && server.ServerOwner == userId)
            {
                invite = new Models.ServerInviteCode() { ServerId = serverId, ExpiresOn = DateTime.MaxValue, ExpiresAfterUses = -1, InviteCode = Guid.NewGuid(), Uses = 0 };
                await _context.ServerInviteCodes.AddAsync(invite);
                await _context.SaveChangesAsync();

            }
            else if (invite == null)
            {
                return BadRequest("You are not the server owner.");
            }
            
            return new JsonResult(new { InviteCode = invite.InviteCode });
        }
    }
}
