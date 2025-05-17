using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

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

        #region Note creation
        [BindProperty]
        public string NewNote { get; set; }
        public async Task<IActionResult> OnPostCreateNoteAsync(string currentPatientId)
        {
            Console.WriteLine("OnPostCreateNoteAsync : " + currentPatientId);
            if (!string.IsNullOrWhiteSpace(NewNote))
            {
                await CreateNote(currentPatientId, NewNote);
            }
            return RedirectToPage();
        }
        public async Task CreateNote(string currentPatientId, string theNote)
        {
            Console.WriteLine("ID client : " + currentPatientId);
            var content = new StringContent($"\"{theNote}\"", Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/medinote/notes/{currentPatientId}/note", content);

            if (!response.IsSuccessStatusCode)
            {//if error
                throw new Exception($"Erreur lors de l'ajout de la note : {response.StatusCode}");
            }
        }
        #endregion
        #region Note destruction
        [BindProperty]
        public string NoteIdToDelete { get; set; }
        public async Task<IActionResult> OnPostDeleteNoteAsync()
        {
            Console.WriteLine("suppression de note : " + NoteIdToDelete);
            if (!string.IsNullOrEmpty(NoteIdToDelete))
            {
                var response = await _httpClient.DeleteAsync($"/medinote/notes/deletenotes/{NoteIdToDelete}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Erreur lors de la suppression de la note : {response.StatusCode}");
                }
            }

            return RedirectToPage(); // Reload page
        }
        #endregion


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
