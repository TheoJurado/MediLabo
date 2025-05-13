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
        public List<NoteDto> Notes { get; set; } = new();

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

            var noteResponse = await _httpClient.GetAsync("/medilabonotes/notes");
            if (noteResponse.IsSuccessStatusCode)
            {
                Notes = await noteResponse.Content.ReadFromJsonAsync<List<NoteDto>>();
            }

            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!" + patientResponse.StatusCode + "/" + noteResponse.StatusCode);
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
            public string Id { get; set; }
        }

        public class NoteDto
        {
            public string Comment { get; set; }
            public string PatientId { get; set; }
        }
    }
}
