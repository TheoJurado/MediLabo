namespace MediNote.Models
{
    public interface INoteRepository
    {
        public Task<IEnumerable<Note>> GetAllNotesAsync();

        public Task<IEnumerable<Note>> GetAllNotesFromPatientByHisId(string patientId);

        public Task<Note?> GetNoteById(string id);

        public void AddNoteToPatient(Note note, string patientId);

        public void DeleteNote(Note note);
    }
}
