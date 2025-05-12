using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Frontend.Pages
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

            var patientResponse = await _httpClient.GetAsync("/medilabo/patients");
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
            public string GenderText { get; set; }
            public string Adress { get; set; }
            public string Phone { get; set; }
        }
    }
}
