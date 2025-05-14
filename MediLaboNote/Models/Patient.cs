using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediLaboNote.Models
{
    public class Patient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public DateTime BirthDay { get; set; }
        public GenderType Gender { get; set; } = GenderType.Other;
        public string GenderText => Gender.ToString();
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
