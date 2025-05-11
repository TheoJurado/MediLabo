using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
    public class Doctor : IdentityUser
    {
        public bool IsOrganizer { get; set; } = false;
    }
}
