using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class PatientNoteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public PatientNoteModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("GatewayClient");
        }

        public List<NoteDto> Notes { get; set; } = new();
        public string Riskof { get; set; } = string.Empty;
        public PatientDto Patient { get; set; } = new PatientDto();

        public async Task OnGetAsync(string id)
        {
            // get note
            var medinoteResponse = await _httpClient.GetAsync($"/medinote/notes/{id}/notes");
            if (medinoteResponse.IsSuccessStatusCode)
            {
                Notes = await medinoteResponse.Content.ReadFromJsonAsync<List<NoteDto>>();
            }

            //get risk
            var risk = await _httpClient.GetAsync($"/riskof/risk/{id}/riskpatient");
            if (risk.IsSuccessStatusCode)
                Riskof = await risk.Content.ReadAsStringAsync();

            //get the patient
            var patientResponse = await _httpClient.GetAsync($"/medilabo/patients/{id}");
            if (patientResponse.IsSuccessStatusCode)
                Patient = await patientResponse.Content.ReadFromJsonAsync<PatientDto>();
        }


        public class NoteDto
        {
            public string Comment { get; set; }
            public string Id { get; set; }
        }
        public class PatientDto
        {
            public string Name { get; set; }
            public string FirstName { get; set; }
            public string Id { get; set; }
        }
    }
}
