using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Frontend.Models;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Frontend.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = clientFactory.CreateClient("GatewayClient");
            _httpContextAccessor = httpContextAccessor;
        }

        //part all data
        public List<DoctorDto> Doctors { get; set; } = new();
        public List<PatientDto> Patients { get; set; } = new();
        public List<NoteDto> Notes { get; set; } = new();

        public async Task LoadAllDataAsync()
        {
            var doctorResponse = await _httpClient.GetAsync("/auth/doctor/all");//GetAllDoctor()
            if (doctorResponse.IsSuccessStatusCode)
            {
                Doctors = await doctorResponse.Content.ReadFromJsonAsync<List<DoctorDto>>();
            }

            var patientResponse = await _httpClient.GetAsync("/medilabo/patients/all");//GetAllPatient()
            if (patientResponse.IsSuccessStatusCode)
            {
                Patients = await patientResponse.Content.ReadFromJsonAsync<List<PatientDto>>();
                if (Patients != null)
                    foreach (PatientDto patient in Patients)
                    {
                        var risk = await _httpClient.GetAsync($"/riskof/risk/{patient.Id}/riskpatient");//GetRiskOfThisPatient()
                        if (risk.IsSuccessStatusCode)
                            patient.Risk = await risk.Content.ReadAsStringAsync();
                    }
            }

            var medinoteResponse = await _httpClient.GetAsync("/medinote/notes/all");//GetAllNotes()
            if (medinoteResponse.IsSuccessStatusCode)
            {
                Notes = await medinoteResponse.Content.ReadFromJsonAsync<List<NoteDto>>();
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadAllDataAsync();
            
            //Check if connected
            if (HttpContext.Session.IsUserLoggedIn())
                return RedirectToPage("/PatientList");
            else
                return RedirectToPage("/Login");/**/

            //return Page();///////////////////////////////////////////////////////////////////
        }
    }
}
