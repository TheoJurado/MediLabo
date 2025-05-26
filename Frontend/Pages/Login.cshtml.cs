using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace Frontend.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = clientFactory.CreateClient("GatewayClient");
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult OnGet()
        {
            //Check if connected
            var _userToken = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(_userToken))
            {//if already connected, go to patient list
                return RedirectToPage("/PatientList");
            }
            
            return Page();
        }
        /////////////////////////////////////////////////
        

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostConnectionAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var loginDto = new DoctorLoginDto
            {
                Email = Input.Email,
                Password = Input.Password
            };

            var response = await _httpClient.PostAsJsonAsync("/auth/doctor/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<DoctorLoginResponseDto>();

                if (result.Success)
                {
                    var session = _httpContextAccessor.HttpContext.Session;

                    session.SetString("JWToken", result.Token);

                    // Extraire les claims depuis le token JWT
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(result.Token);

                    var isOrganizer = jwt.Claims.FirstOrDefault(c => c.Type == "IsOrganizer")?.Value ?? "false";
                    session.SetString("IsOrganizer", isOrganizer);
                    session.SetString("IsLoggedIn", "true");

                    return RedirectToPage("/PatientList");
                }

                ErrorMessage = "Identifiants incorrects.";
            }
            else
            {
                ErrorMessage = "Erreur lors de la connexion.";
            }
            return Page();
        }
    }
}
