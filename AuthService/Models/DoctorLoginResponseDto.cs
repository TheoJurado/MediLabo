namespace AuthService.Models
{
    public class DoctorLoginResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public bool IsOrganizer { get; set; }
    }
}
