using ChatItUp.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatItUp.Pages
{
    public class LeaveServerModel : PageModel
    {
        [BindProperty(SupportsGet = true)] // Add SupportsGet = true to allow binding on GET requests
        public Guid? ServerId { get; set; } = Guid.Empty;

        public string Name { get; set; } = "";
        public string CurrentUrl => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

        [BindProperty]
        bool ConfirmDelete { get; set; }

        Context.CIUDataDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ChatService _chatService;

        public LeaveServerModel(Context.CIUDataDbContext context, IHubContext<ChatHub> hubContext, ChatService chatService)
        {
            _context = context;
            _hubContext = hubContext;
            _chatService = chatService;
        }

        public async void OnGet()
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
            var server = _context.Servers.FirstOrDefault(s => s.Id == ServerId);
            if (server == null)
            {
                ModelState.AddModelError(string.Empty, "Coudln't find that server.");
            }
            else
            {
                Name = server.Name;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

            if(!ServerId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Server ID not set.");
                return BadRequest("Server ID not set.");
            }
            var server = _context.Servers.First(s => s.Id == ServerId);
            var serveruser = _context.UserServers.FirstOrDefault(s => s.UserId == userId && s.ServerId == ServerId);
            if(server.ServerOwner == userId)
            {
                ModelState.AddModelError(string.Empty, "Can't leave server while you own it.");
                return BadRequest("You can't leave a server you currently own.");
            }
            else if (serveruser == null)
            {
                ModelState.AddModelError(string.Empty, "You aren't a member of that server.");
                return BadRequest("You aren't a member of that server.");
            }
            else
            {
                _context.UserServers.Remove(serveruser);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.Group("server_" + serveruser.ServerId.ToString()).SendAsync("UserLeft", serveruser.ServerId, serveruser.UserId);

                await _hubContext.Clients.User(userId.ToString()).SendAsync("RemoveServer", serveruser.ServerId);
                
                return new JsonResult(new { Message = "Left server." });
            }
        }
    }
}
