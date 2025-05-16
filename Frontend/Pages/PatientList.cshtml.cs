using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class PatientListModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public PatientListModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("GatewayClient");
        }

        public List<PatientDto> Patients { get; set; } = new();
        public List<NoteDto> Notes { get; set; } = new();

        public async Task OnGetAsync()
        {
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
