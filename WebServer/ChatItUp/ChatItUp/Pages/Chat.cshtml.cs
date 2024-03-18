using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChatItUp.Pages
{
    [Authorize]
    public class ChatModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
