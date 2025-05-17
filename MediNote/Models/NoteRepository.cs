using MediNote.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MediNote.Models
{
    public class NoteRepository : INoteRepository
    {
        private readonly IMongoCollection<Note> _notes;

        public NoteRepository(IOptions<MongoDbSettings> settings, IMongoClient client)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _notes = database.GetCollection<Note>("Notes");
        }

        public async Task<IEnumerable<Note>> GetAllNotesAsync()
        {
            var allNotes = await _notes.Find(n => true).ToListAsync();

            return allNotes;
        }

        public async Task<IEnumerable<Note>> GetAllNotesFromPatientByHisId(string patientId)
        {
            var filter = Builders<Note>.Filter.Eq(n => n.PatientId, patientId);
            return await _notes.Find(filter).ToListAsync();
        }

        public async Task<Note?> GetNoteById(string id)
        {
            return await _notes.Find(n => n.Id == id).FirstOrDefaultAsync();
        }



        public void AddNoteToPatient(Note note, string patientId)
        {
            note.PatientId = patientId;
            _notes.InsertOne(note);
        }

        public void DeleteNote(Note note)
        {
            var filter = Builders<Note>.Filter.Eq(n => n.Id, note.Id);
            _notes.DeleteOne(filter);
        }
    }
}
