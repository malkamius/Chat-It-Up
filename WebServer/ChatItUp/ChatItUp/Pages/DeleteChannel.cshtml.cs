using ChatItUp.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatItUp.Pages
{
    public class DeleteChannelModel : PageModel
    {
        [BindProperty(SupportsGet = true)] // Add SupportsGet = true to allow binding on GET requests
        public Guid? ChannelId { get; set; } = Guid.Empty;

        public string Name { get; set; } = "";
        public string CurrentUrl => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

        [BindProperty]
        bool ConfirmDelete { get; set; }

        Context.CIUDataDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ChatService _chatService;

        public DeleteChannelModel(Context.CIUDataDbContext context, IHubContext<ChatHub> hubContext, ChatService chatService)
        {
            _context = context;
            _hubContext = hubContext;
            _chatService = chatService;
        }

        public async void OnGet()
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

            if(!ChannelId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "No channel was specified.");
                return;
            }

            var channel = (from c in _context.ServerChannels join s in _context.Servers on c.ServerId equals s.Id where s.ServerOwner == userId && c.Id == ChannelId.Value select c).FirstOrDefault();

            if (channel == null)
            {
                ModelState.AddModelError(string.Empty, "You don't own that server.");
            }
            else
            {
                Name = channel.Name;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
            
            if(!ChannelId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Server ID not set.");
                return BadRequest("Server ID not set.");
            }
            var channel = (from c in _context.ServerChannels join s in _context.Servers on c.ServerId equals s.Id where s.ServerOwner == userId && c.Id == ChannelId.Value select c).FirstOrDefault();

            if (channel == null)
            {
                ModelState.AddModelError(string.Empty, "You don't own that server.");
                return BadRequest("You don't own that server.");
            }
            else
            {
                channel.DeletedOn = DateTime.Now;
                _context.Update(channel);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.Users(from usid in (await _chatService.GetUsersForServer(ChannelId.Value)) select usid.ToString()).SendAsync("ChannelRemoved", ChannelId.Value, channel.ServerId);

                return new JsonResult(new { Message = "Channel deleted." });
            }
        }
    }
}
