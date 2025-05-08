using MediLabo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MediLabo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        //temporaire pour check les docs >>les suivant seront anoté d'un //#
        private readonly UserManager<Doctor> _userManager;

        public PatientsController(IPatientRepository patientRepository, UserManager<Doctor> userManager)
        {
            _patientRepository = patientRepository;
            _userManager = userManager;//#
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
        {
            var patients = await _patientRepository.GetAllPatientAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatientById(string id)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpPost]
        //[SwaggerRequestExample(typeof(Patient), typeof(PatientExample))]
        public ActionResult AddPatient([FromBody] Patient patient)
        {
            _patientRepository.AddPatient(patient);
            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(string id)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _patientRepository.DeletePatient(patient);
            return NoContent();
        }

        [HttpGet("{id}/notes")]
        public async Task<ActionResult<IEnumerable<Note>>> GetPatientNotes(string id)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            var notes = await _patientRepository.GetAllNotesFromPatientByHisId(id);
            return Ok(notes);
        }

        [HttpPost("{id}/notes")]
        public async Task<ActionResult> AddNoteToPatient(string id, [FromBody] Note note)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _patientRepository.AddNoteToPatient(note, patient);
            return Ok();
        }

        [HttpDelete("notes/{noteId}")]
        public ActionResult DeleteNoteFromAnyPatient(string noteId)
        {
            var note = new Note { Id = noteId };
            _patientRepository.DeleteNote(note);
            return NoContent();
        }/**/

        [HttpGet("all")]//#
        public IActionResult GetAllDoctors()
        {
            var doctors = _userManager.Users.ToList();
            return Ok(doctors);
        }
    }
}
