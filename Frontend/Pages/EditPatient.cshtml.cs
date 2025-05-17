using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;

namespace Frontend.Pages
{
    public class EditPatientModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditPatientModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("GatewayClient");
        }

        [BindProperty]
        public Patient Patient { get; set; } = new Patient();

        public bool IsEditMode => !string.IsNullOrEmpty(Patient.Id);

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Patient = await _patientService.GetPatientByIdAsync(id);
                if (Patient == null)
                {
                    return NotFound();
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (string.IsNullOrEmpty(Patient.Id))
            {
                // Ajouter un nouveau patient
                await _patientService.CreatePatientAsync(Patient);
            }
            else
            {
                // Mettre à jour un patient existant
                await _patientService.UpdatePatientAsync(Patient.Id, Patient);
            }

            return RedirectToPage("/Patients/Index"); // Rediriger vers la liste
        }
    }

    public class Patient
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public DateTime BirthDay { get; set; }
        public GenderType Gender { get; set; } = GenderType.Other;
        public string GenderText => Gender.ToString();
        public string? Adress { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
    }

    public enum GenderType
    {
        Man,
        Woman,
        Other
    }
}
