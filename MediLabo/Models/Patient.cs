using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediLabo.Models
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
        public string Adress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;


        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }

    public enum GenderType
    {
        Man,
        Woman,
        Other
    }
}
