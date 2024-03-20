using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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


        public UseInviteModel(Context.CIUDataDbContext context)
        {
            _context = context;
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
                    if (invite.ExpiresOn < DateTime.Now)
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
                            Successful = true;
                        }
                    }
                }
            }
        }
    }
}
