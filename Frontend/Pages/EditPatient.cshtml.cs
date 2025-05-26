using Frontend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Frontend.Pages
{
    public class EditPatientModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditPatientModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = clientFactory.CreateClient("GatewayClient");
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public Patient Patient { get; set; } = new Patient();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            //Check if connected
            var _userToken = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(_userToken))
                return RedirectToPage("/Login");//if not connected, go to connection page
            var isOrganizer = _httpContextAccessor.HttpContext.Session.GetString("IsOrganizer");
            if (isOrganizer != "true")
                return RedirectToPage("/PatientList");//if not an organizator, go to PatientList


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
            Console.WriteLine("Patient ID     : " + id);
            Console.WriteLine("Patient name   : " + Patient.Name);
            Console.WriteLine("Patient adress : " + Patient.Adress);
            //get token
            var _userToken = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(_userToken))
                throw new InvalidOperationException("Utilisateur non authentifié ou token manquant.");
            Console.WriteLine("UserToken : " + _userToken);

            //check model
            if (!ModelState.IsValid)
                return Page();
            Console.WriteLine("Model is valid");


            var jsonPatient = JsonSerializer.Serialize(Patient);
            var content = new StringContent(jsonPatient, Encoding.UTF8, "application/json");

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/medilabo/patients/{id}"))//UpdatePatient(id, updatedPatient)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userToken);
                requestMessage.Content = content;

                var response = await _httpClient.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                {//if error
                    Console.WriteLine("response is bad");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erreur lors de la mise a jour du patient : {response.StatusCode}, Details: {errorContent}");
                }
            }
            Console.WriteLine("response is god");

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
