using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            Console.WriteLine("Déconnexion frontend (via Razor Page)");
            return RedirectToPage("/Login");
        }
    }
}
