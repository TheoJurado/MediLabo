using MediLabo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MediLabo.Controllers
{
    [ApiController]
    [Route("patientapi/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientsController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
        {
            Console.WriteLine("Asking for all patients");
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

        [Authorize] // Connected
        [Authorize(Policy = "OrganizerOnly")] // IsOrganizer
        [HttpPost("add")]
        public ActionResult AddPatient([FromBody] Patient patient)
        {
            _patientRepository.AddPatient(patient);
            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
        }

        [Authorize] // Connected
        [Authorize(Policy = "OrganizerOnly")] // IsOrganizer
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePatient(string id, [FromBody] Patient updatedPatient)
        {
            Console.WriteLine("--- User Claims in UpdatePatient ---");
            if (User?.Claims != null)
            {
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}, Issuer: {claim.Issuer}, OriginalIssuer: {claim.OriginalIssuer}, ValueType: {claim.ValueType}");
                }
            }
            else
            {
                Console.WriteLine("User.Claims is null or User is null.");
            }
            Console.WriteLine("------------------------------------");
            // Vérification manuelle (pour votre compréhension)
            bool hasClaimDirectly = User.HasClaim("scope", "organizer_access");
            Console.WriteLine($"User.HasClaim(\"scope\", \"organizer_access\"): {hasClaimDirectly}");

            bool hasClaimWithTypeOnly = User.Claims.Any(c => c.Type == "scope");
            Console.WriteLine($"User has any claim with type 'scope': {hasClaimWithTypeOnly}");

            if (hasClaimWithTypeOnly)
            {
                var scopeClaims = User.Claims.Where(c => c.Type == "scope").ToList();
                foreach (var sc in scopeClaims)
                {
                    Console.WriteLine($"Found scope claim - Value: '{sc.Value}' (Length: {sc.Value.Length}), Is 'organizer_access'?: {sc.Value == "organizer_access"}");
                    // Parfois, des espaces invisibles peuvent causer des problèmes
                    if (sc.Value != "organizer_access")
                    {
                        Console.WriteLine($"Value is not 'organizer_access'. Hex: {BitConverter.ToString(Encoding.UTF8.GetBytes(sc.Value))}");
                    }
                }
            }
            //End of Logs

            var existingPatient = await _patientRepository.GetPatientByIdAsync(id);
            if (existingPatient == null)
            {
                return NotFound();
            }

            updatedPatient.Id = id; // Make sure ID still the same
            await _patientRepository.UpdatePatientAsync(id, updatedPatient);
            return NoContent();
        }

        [Authorize] // Connected
        [Authorize(Policy = "OrganizerOnly")] // IsOrganizer
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
    }
}
