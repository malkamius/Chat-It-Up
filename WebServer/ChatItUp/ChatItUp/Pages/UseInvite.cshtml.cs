using ChatItUp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatItUp.Pages
{
    [Authorize]
    public class UseInviteModel : PageModel
    {
        [BindProperty(SupportsGet = true)] // Add SupportsGet = true to allow binding on GET requests
        public Guid? InviteCode { get; set; } = Guid.Empty;

        private Context.CIUDataDbContext _context;

        [BindProperty]
        public bool Successful { get; set; }

        [BindProperty]
        public bool LinkExpired { get; set; }


        [BindProperty]
        public bool AlreadyMember { get; set; }

        private IHubContext<ChatHub> _hubContext;

        public UseInviteModel(Context.CIUDataDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task OnGetAsync()
        {
            if(InviteCode.HasValue && InviteCode.Value != Guid.Empty)
            {
                Guid userId = Guid.Empty;
                Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

                var invite = _context.ServerInviteCodes.FirstOrDefault(invite => invite.InviteCode == InviteCode);
                
                if(invite != null) 
                {
                    var server = _context.Servers.FirstOrDefault(s => s.Id == invite.ServerId);
                    var user = _context.Users.First(u => u.Id == userId);
                    if (invite.ExpiresOn < DateTime.Now || server == null || server.DeletedOn.HasValue)
                    {
                        Successful = false;
                        LinkExpired = true;
                    }
                    else
                    {
                        if (_context.UserServers.Any(s => s.UserId == userId && s.ServerId == invite.ServerId))
                        {
                            AlreadyMember = true;
                        }
                        else
                        {
                            invite.Uses++;
                            _context.ServerInviteCodes.Update(invite);
                            await _context.UserServers.AddAsync(new Models.UserServer() { UserId = userId, JoinedOn = DateTime.Now, ServerId = invite.ServerId });
                            await _context.SaveChangesAsync();
                            if (ChatHub.UserConnections.TryGetValue(userId, out var connections))
                            {
                                foreach (var connection in connections)
                                    await _hubContext.Groups.AddToGroupAsync(connection, "server_" + server.Id.ToString());
                            }
                            await _hubContext.Clients.Group("server_" + server.Id.ToString()).SendAsync("UserConnected", server.Id, userId, user.DisplayName ?? user.EmailAddress, false);
                            await _hubContext.Clients.User(userId.ToString()).SendAsync("ServerAdded", server.Id, server.Name, server.ServerOwner == userId);

                            Successful = true;
                        }
                        
                    }
                }
            }
        }
    }
}
