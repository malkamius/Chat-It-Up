using ChatItUp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Tls;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.Security.Claims;

namespace ChatItUp.Pages
{
    [Authorize]
    public class CreateChannelModel : PageModel
    {
        public string CurrentUrl => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "Server Id is required.")]
        public Guid ServerId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Channel name is required.")]
        public string Name { get; set; }

        
        Context.CIUDataDbContext _context;
        IHubContext<ChatHub> _hubContext;
        ChatService _chatService;
        public CreateChannelModel(Context.CIUDataDbContext context, IHubContext<ChatHub> hubContext, ChatService chatService)
        {
            _context = context;
            _hubContext = hubContext;
            _chatService = chatService;
        }

        public void OnGet()
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

        }

        public async Task<IActionResult> OnPostAsync()
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
            
            if(ServerId ==  Guid.Empty)
            {
                ModelState.AddModelError("ServerId", "Server Id not provided.");
                //return Page();
                return BadRequest("No server id was supplied.");
            }
            var server = _context.Servers.FirstOrDefault(s => s.Id == ServerId);

            if(server == null || server.ServerOwner != userId)
            {
                ModelState.AddModelError("ServerId", "You do not have permission.");
                //return Page();
                return BadRequest("You do not have permission.");
            }
           
            var channelExists = _context.ServerChannels.Any(c => c.ServerId == server.Id && c.Name.Equals(Name.Trim()));
            
            if(channelExists)
            {
                ModelState.AddModelError(string.Empty, "A channel with that name already exists.");
                //return Page();
                return BadRequest("A channel with that name already exists.");
            }
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "There was an error with the data submitted.");
                //return Page();
                return BadRequest("There was an error with the data submitted.");
            }

            var channel = new Models.ServerChannel() { Id = Guid.NewGuid(), ServerId = ServerId, Name = Name };
            await _context.ServerChannels.AddAsync(channel);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Users(from usid in (await _chatService.GetUsersForServer(ServerId)) select usid.ToString()).SendAsync("ChannelAdded", ServerId, channel.Id);

            return new JsonResult(new {ServerId = channel.ServerId, ChannelId = channel.Id});
        }
    }
}