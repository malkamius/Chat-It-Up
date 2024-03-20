using ChatItUp.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatItUp.Pages
{
    public class DeleteServerModel : PageModel
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

        public DeleteServerModel(Context.CIUDataDbContext context, IHubContext<ChatHub> hubContext, ChatService chatService)
        {
            _context = context;
            _hubContext = hubContext;
            _chatService = chatService;
        }

        public async void OnGet()
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
            var server = _context.Servers.FirstOrDefault(s => s.ServerOwner == userId && s.Id == ServerId);
            if (server == null)
            {
                ModelState.AddModelError(string.Empty, "You don't own that server.");
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
            var server = _context.Servers.FirstOrDefault(s => s.ServerOwner == userId && s.Id == ServerId);

            if (server == null)
            {
                ModelState.AddModelError(string.Empty, "You don't own that server.");
                return BadRequest("You don't own that server.");
            }
            else
            {
                //_context.UserServers.RemoveRange(_context.UserServers.Where(s => s.ServerId == ServerId));
                //await _context.SaveChangesAsync();
                //_context.Servers.Remove(server);
                //await _context.SaveChangesAsync();
                server.DeletedOn = DateTime.Now;
                _context.Update(server);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.Users(from usid in (await _chatService.GetUsersForServer(ServerId.Value)) select usid.ToString()).SendAsync("ServerRemoved", ServerId.Value);

                return new JsonResult(new { Message = "Server deleted." });
            }
        }
    }
}
