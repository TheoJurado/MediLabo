using Microsoft.AspNetCore.Identity;

namespace Frontend.Models
{
    public class NoteDto
    {
        public string Comment { get; set; }
        public string PatientId { get; set; }
    }
}
