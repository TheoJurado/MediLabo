using Microsoft.AspNetCore.Identity;

namespace MediLabo.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public DateTime BirthDay { get; set; }
        public GenderType Gender { get; set; } = GenderType.Other;
        public string Adress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public enum GenderType
    {
        Man,
        Woman,
        Other
    }
}
