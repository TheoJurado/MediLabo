namespace MediLabo.Models
{
    public interface IPatientRepository
    {
        public Task<IEnumerable<Patient>> GetAllPatientAsync();

        public Task<Patient> GetPatientByIdAsync(string id);



        public void AddPatient(Patient patient);
        Task UpdatePatientAsync(string id, Patient updatedPatient);
        public void DeletePatient(Patient patient);
    }
}
