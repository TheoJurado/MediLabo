using Frontend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace Frontend.Pages
{
    public class EditPatientModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<DoctorDto> _userManager;

        public EditPatientModel(IHttpClientFactory clientFactory, UserManager<DoctorDto> userManager)
        {
            _httpClient = clientFactory.CreateClient("GatewayClient");
            _userManager = userManager;
        }

        [BindProperty]
        public Patient Patient { get; set; } = new Patient();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            //Check if connected
            if (!HttpContext.Session.IsUserLoggedIn())
                return RedirectToPage("/Login");
            var user = await _userManager.GetUserAsync(User);
            if (user is DoctorDto doctor && !doctor.IsOrganizer)
                return RedirectToPage("/PatientList");

            if (!string.IsNullOrEmpty(id))
            {
                var patientResponse = await _httpClient.GetAsync($"/medilabo/patients/{id}");//GetThisPatient(id)
                if (patientResponse.IsSuccessStatusCode)
                    Patient = await patientResponse.Content.ReadFromJsonAsync<Patient>();
                if (Patient == null)
                    return NotFound();
            }
            else
                return NotFound();//no ID given
            return Page();
        }

        public async Task<IActionResult> OnPostEditPatientAsync(string id)
        {
            Console.WriteLine("OnPostEditPatientAsync started");
            Console.WriteLine("Patient ID : " + id);
            Console.WriteLine("Patient name : " + Patient.Name);
            Console.WriteLine("Patient adress : " + Patient.Adress);
            if (!ModelState.IsValid)
                return Page();

            var response = await _httpClient.PutAsJsonAsync($"/medilabo/patients/{id}", Patient);//UpdatePatient(id, updatedPatient)

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Échec de la mise à jour du patient.");
                return Page();
            }


            return RedirectToPage("/PatientList"); // Return to Patient List
        }
    }

    public class Patient
    {
        public string? Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public DateTime BirthDay { get; set; }
        [Required]
        public GenderType Gender { get; set; } = GenderType.Other;
        public string GenderText => Gender.ToString();
        public string? Adress { get; set; } = string.Empty;
        [Phone]
        public string? Phone { get; set; } = string.Empty;
    }

    public enum GenderType
    {
        Man,
        Woman,
        Other
    }
}
