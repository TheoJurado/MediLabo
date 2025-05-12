using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendApp.Views.Home
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("GatewayClient");
        }

        public List<DoctorDto> Doctors { get; set; } = new();
        public List<PatientDto> Patients { get; set; } = new();

        public async Task OnGetAsync()
        {
            var doctorResponse = await _httpClient.GetAsync("/auth/doctor/all");
            if (doctorResponse.IsSuccessStatusCode)
            {
                Doctors = await doctorResponse.Content.ReadFromJsonAsync<List<DoctorDto>>();
            }

            var patientResponse = await _httpClient.GetAsync("/medilabo/patient");
            if (patientResponse.IsSuccessStatusCode)
            {
                Patients = await patientResponse.Content.ReadFromJsonAsync<List<PatientDto>>();
            }
        }

        public class DoctorDto
        {
            public string Email { get; set; }
            public bool IsOrganizer { get; set; }
        }

        public class PatientDto
        {
            public string Name { get; set; }
            public string FirstName { get; set; }
            public DateTime BirthDay { get; set; }
            public string Gender { get; set; }
            public string Adress { get; set; }
            public string Phone { get; set; }
        }
    }
}
