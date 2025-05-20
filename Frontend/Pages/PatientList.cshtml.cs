using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Frontend.Models;
using Microsoft.AspNetCore.Identity;

namespace Frontend.Pages
{
    public class PatientListModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<DoctorDto> _userManager;

        public PatientListModel(IHttpClientFactory clientFactory, UserManager<DoctorDto> userManager)
        {
            _httpClient = clientFactory.CreateClient("GatewayClient");
            _userManager = userManager;
        }

        public List<PatientDto> Patients { get; set; } = new();
        public List<NoteDto> Notes { get; set; } = new();
        public string ErrorMessage { get; set; }
        public bool IsUserOrganizer { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //Check if connected
            if (!HttpContext.Session.IsUserLoggedIn())
                return RedirectToPage("/Login");
            //check if organizer
            var user = await _userManager.GetUserAsync(User);
            if (user is DoctorDto doctor)
                IsUserOrganizer = doctor.IsOrganizer;

            //get all patient
            var patientResponse = await _httpClient.GetAsync("/medilabo/patients/all");
            if (patientResponse.IsSuccessStatusCode)
            {
                Patients = await patientResponse.Content.ReadFromJsonAsync<List<PatientDto>>();
                if (Patients != null)
                    foreach (PatientDto patient in Patients)
                    {//foreach patient, get calculated risk
                        var risk = await _httpClient.GetAsync($"/riskof/risk/{patient.Id}/riskpatient");
                        if (risk.IsSuccessStatusCode)
                            patient.Risk = await risk.Content.ReadAsStringAsync();
                    }
            }
            else
            {
                ErrorMessage = "Impossible de charger les patients.";
            }

            return Page();
        }


        public class PatientDto
        {
            public string Name { get; set; }
            public string FirstName { get; set; }
            public DateTime BirthDay { get; set; }
            public string GenderText { get; set; }
            public string Adress { get; set; }
            public string Phone { get; set; }
            public string Id { get; set; }
            public string Risk { get; set; }
        }

        public class NoteDto
        {
            public string Comment { get; set; }
            public string PatientId { get; set; }
        }
    }
}
