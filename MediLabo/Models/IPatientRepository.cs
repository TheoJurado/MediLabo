using Microsoft.EntityFrameworkCore;

namespace MediLabo.Models
{
    public interface IPatientRepository
    {
        public IEnumerable<Patient> GetAllPatient();

        public Patient GetPatientById(int id);

        public IEnumerable<Note> GetAllNotesFromPatientByHisId(int id);



        public void AddPatient(Patient patient);
        public void DeletePatient(Patient patient);

        public void AddNoteToPatient(Note note, Patient patient);

        public void DeleteNote(Note note);
    }
}
