using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediLaboNote.Models
{
    public class Note
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateOnly? Date { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PatientId { get; set; } = string.Empty;
    }
}
