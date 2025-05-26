using Microsoft.AspNetCore.Identity;

namespace Frontend.Models
{
    public class DoctorDto : IdentityUser
    {
        public string Email { get; set; }
        public bool IsOrganizer { get; set; }
    }
}
