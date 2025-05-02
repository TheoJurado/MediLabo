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
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _patients = database.GetCollection<Patient>("Patients");
        }


        public IEnumerable<Patient> GetAllPatient() 
        {
            IEnumerable<Patient> allPatient = _context.Patient.Where(p => p.Id > 0);

            return allPatient.ToList();
        }

        public Patient GetPatientById(int id) 
        {
            List<Patient> Patients = GetAllPatient().ToList();
            return Patients.Find(p => p.Id == id);
        }

        public IEnumerable<Note> GetAllNotesFromPatientByHisId(int id)
        {
            Patient patient = GetPatientById(id);
            return patient.Notes;
        }



        public void AddPatient(Patient patient)
        {
            _context.Patient.Add(patient);
            _context.SaveChanges();
        }
        public void DeletePatient(Patient patient) 
        {
            _context.Remove(patient);
            _context.SaveChanges();
        }

        public void AddNoteToPatient(Note note,Patient patient)
        {
            patient.Notes.Add(note);
            _context.SaveChanges();
        }

        public void DeleteNote(Note note)
        {
            _context.Remove(note);
            _context.SaveChanges();
        }
    }
}
