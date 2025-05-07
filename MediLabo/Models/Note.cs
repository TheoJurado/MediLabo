using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediLabo.Models
{
    public class Note
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateOnly Date { get; set; }

        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }
    }
}
