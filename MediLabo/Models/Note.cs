namespace MediLabo.Models
{
    public class Note
    {
        int Id { get; set; }
        string Comment { get; set; } = string.Empty;
        DateOnly Date { get; set; }

        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }
    }
}
