using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatItUp.Services
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GetServerInfo : ControllerBase
    {
        private Context.CIUDataDbContext _context;

        public GetServerInfo(Context.CIUDataDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers(Guid serverId)
        {
            var users = await _context.UserServers
                                      .Where(us => us.ServerId == serverId)
                                      .Select(us => new { Id = us.UserId, DisplayName = us.User.DisplayName ?? us.User.EmailAddress, Status = us.User.Status ?? "Offline", IsOwner = us.Server.ServerOwner == us.UserId })
                                      .ToListAsync();

            return new JsonResult(users);
        }
    }
}
