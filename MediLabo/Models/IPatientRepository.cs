using Microsoft.EntityFrameworkCore;

namespace MediLabo.Models
{
    public interface IPatientRepository
    {
        public Task<IEnumerable<Patient>> GetAllPatientAsync();

        public Task<Patient> GetPatientByIdAsync(int id);

        public Task<IEnumerable<Note>> GetAllNotesFromPatientByHisId(int id);



        public void AddPatient(Patient patient);
        public void DeletePatient(Patient patient);

        public void AddNoteToPatient(Note note, Patient patient);

        public void DeleteNote(Note note);
    }
}
