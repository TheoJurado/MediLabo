using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public IActionResult OnGet()
        {
            // Delet session data
            var session = _httpContextAccessor.HttpContext.Session;
            session.Remove("JWToken");
            session.Remove("IsOrganizer");
            session.Remove("IsLoggedIn");

            // To make sure
            session.Clear();

            return RedirectToPage("/Login");
        }
    }
}
