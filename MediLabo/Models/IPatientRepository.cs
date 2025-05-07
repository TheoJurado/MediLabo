using Microsoft.EntityFrameworkCore;

namespace MediLabo.Models
{
    public interface IPatientRepository
    {
        public Task<IEnumerable<Patient>> GetAllPatientAsync();

        public Task<Patient> GetPatientByIdAsync(string id);

        public Task<IEnumerable<Note>> GetAllNotesFromPatientByHisId(string id);



        public void AddPatient(Patient patient);
        public void DeletePatient(Patient patient);

        public void AddNoteToPatient(Note note, Patient patient);

        public void DeleteNote(Note note);
    }
}
