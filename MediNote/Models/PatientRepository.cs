using MediNote.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MediNote.Models
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IMongoCollection<Patient> _patients;

        public PatientRepository(IOptions<MongoDbSettings> settings, IMongoClient client)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _patients = database.GetCollection<Patient>("Patients");
        }


        public async Task<IEnumerable<Patient>> GetAllPatientAsync()
        {
            var allPatients = await _patients.Find(p => true).ToListAsync();

            return allPatients;
        }

        public async Task<Patient> GetPatientByIdAsync(string id)
        {
            return await _patients.Find(x => x.Id == id).FirstOrDefaultAsync();
        }



        public void AddPatient(Patient patient)
        {
            _patients.InsertOne(patient);
        }
        public void DeletePatient(Patient patient)
        {
            _patients.DeleteOne(p => p.Id == patient.Id);
        }
    }
}
