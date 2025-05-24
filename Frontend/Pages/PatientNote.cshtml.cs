using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;

namespace Frontend.Pages
{
    public class PatientNoteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientNoteModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = clientFactory.CreateClient("GatewayClient");
            _httpContextAccessor = httpContextAccessor;
        }

        public List<NoteDto> Notes { get; set; } = new();
        public string Riskof { get; set; } = string.Empty;
        public PatientDto Patient { get; set; } = new PatientDto();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            //Check if connected
            var _userToken = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(_userToken))
            {//if not connected, go to connection page
                return RedirectToPage("/Login");
            }

            // get note
            var medinoteResponse = await _httpClient.GetAsync($"/medinote/notes/{id}/notes");//GetAllNoteForThisPatient(id)
            if (medinoteResponse.IsSuccessStatusCode)
            {
                Notes = await medinoteResponse.Content.ReadFromJsonAsync<List<NoteDto>>();
            }

            //get risk
            var risk = await _httpClient.GetAsync($"/riskof/risk/{id}/riskpatient");//GetRiskForThisPatient(id)
            if (risk.IsSuccessStatusCode)
                Riskof = await risk.Content.ReadAsStringAsync();

            //get the patient
            var patientResponse = await _httpClient.GetAsync($"/medilabo/patients/{id}");//GetThisPatient(id)
            if (patientResponse.IsSuccessStatusCode)
                Patient = await patientResponse.Content.ReadFromJsonAsync<PatientDto>();

            return Page();
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
            //get token
            var _userToken = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(_userToken))
            {
                throw new InvalidOperationException("Utilisateur non authentifié ou token manquant.");
            }
            Console.WriteLine("UserToken : " + _userToken);

            var content = new StringContent($"\"{theNote}\"", Encoding.UTF8, "application/json");
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"/medinote/notes/{currentPatientId}/note"))//AddNoteToPatient()
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userToken);
                requestMessage.Content = content;

                var response = await _httpClient.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                {//if error
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erreur lors de l'ajout de la note : {response.StatusCode}, Détails: {errorContent}");
                }
            }
        }
        #endregion
        #region Note destruction
        [BindProperty]
        public string NoteIdToDelete { get; set; }
        public async Task<IActionResult> OnPostDeleteNoteAsync()
        {
            Console.WriteLine("suppression de note : " + NoteIdToDelete);
            //get token
            var _userToken = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(_userToken))
            {
                throw new InvalidOperationException("Utilisateur non authentifié ou token manquant.");
            }
            Console.WriteLine("UserToken : " + _userToken);

            if (!string.IsNullOrEmpty(NoteIdToDelete))
            {
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/medinote/notes/deletenotes/{NoteIdToDelete}"))//DeleteNote()
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userToken);

                    var response = await _httpClient.SendAsync(requestMessage);

                    if (!response.IsSuccessStatusCode)
                    {//if error
                        var errorContent = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Erreur lors de la suppression de la note : {response.StatusCode}, Détails: {errorContent}");
                    }
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
