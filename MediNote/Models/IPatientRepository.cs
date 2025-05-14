namespace MediNote.Models
{
    public interface IPatientRepository
    {
        public Task<IEnumerable<Patient>> GetAllPatientAsync();

        public Task<Patient> GetPatientByIdAsync(string id);



        public void AddPatient(Patient patient);
        public void DeletePatient(Patient patient);
    }
}
