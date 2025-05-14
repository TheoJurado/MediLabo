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

            var patientResponse = await _httpClient.GetAsync("/medilabo/patients/all");
            if (patientResponse.IsSuccessStatusCode)
            {
                Patients = await patientResponse.Content.ReadFromJsonAsync<List<PatientDto>>();
            }
            /*
            var labonoteResponse = await _httpClient.GetAsync("/medilabonotes/notes");
            if (labonoteResponse.IsSuccessStatusCode)
            {
                /*Notes = await labonoteResponse.Content.ReadFromJsonAsync<List<NoteDto>>();
            }*/

            var medinoteResponse = await _httpClient.GetAsync("/medinote/notes/all");
            if (medinoteResponse.IsSuccessStatusCode)
            {
                Notes = await medinoteResponse.Content.ReadFromJsonAsync<List<NoteDto>>();
            }

            Console.WriteLine("Doctor > " + doctorResponse.StatusCode);
            Console.WriteLine("Patient > " + patientResponse.StatusCode);
            Console.WriteLine("Notes > " + medinoteResponse.StatusCode);
            var content = await medinoteResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Contenu de la réponse Note : " + content);
            if (medinoteResponse.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var jsonContent = await medinoteResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Erreur JSON : " + jsonContent);
            }
            foreach (var header in medinoteResponse.Headers)
            {
                Console.WriteLine($"{header.Key} : {string.Join(",", header.Value)}");
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
            public string Id { get; set; }
        }

        public class NoteDto
        {
            public string Comment { get; set; }
            public string PatientId { get; set; }
        }
    }
}
