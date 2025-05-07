using MediLabo.Data;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace MediLabo.Models
{
    public class PatientRepository :IPatientRepository
    {
        //private readonly ApplicationDbContext _context;
        private readonly IMongoCollection<Patient> _patients;

        /*public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }*/
        public PatientRepository(IOptions<MongoDbSettings> settings, IMongoClient client)
        {
            //var client = new MongoClient(settings.Value.ConnectionString);
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

        public async Task<IEnumerable<Note>> GetAllNotesFromPatientByHisId(string id)
        {
            Patient patient = await GetPatientByIdAsync(id);
            return patient.Notes;
        }



        public void AddPatient(Patient patient)
        {
            _patients.InsertOne(patient);
        }
        public void DeletePatient(Patient patient) 
        {
            _patients.DeleteOne(p => p.Id == patient.Id);
        }

        public void AddNoteToPatient(Note note,Patient patient)
        {
            var update = Builders<Patient>.Update.Push(p => p.Notes, note);
            _patients.UpdateOne(p => p.Id == patient.Id, update);
        }

        public void DeleteNote(Note note)
        {
            var filter = Builders<Patient>.Filter.ElemMatch(p => p.Notes, n => n.Id == note.Id);
            var update = Builders<Patient>.Update.PullFilter(p => p.Notes, n => n.Id == note.Id);
            _patients.UpdateOne(filter, update);
        }
    }
}
