using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("authapi/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly UserManager<Doctor> _userManager;
        private readonly IConfiguration _configuration;

        public DoctorController(UserManager<Doctor> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet("all")]
        public IActionResult GetAllDoctors()
        {
            var doctors = _userManager.Users.ToList();
            return Ok(doctors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DoctorLoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                Console.WriteLine("ID-PW invalide ! Email : " + dto.Email + " > PW : " + dto.Password);
                return Unauthorized(new { success = false });
            }

            var token = GenerateJwtToken(user);
            Console.WriteLine("login succed ! Token : " + token);
            return Ok(new DoctorLoginResponseDto
            {
                Success = true,
                Token = token,
                IsOrganizer = user.IsOrganizer
            });
        }

        private string GenerateJwtToken(Doctor user)
        {
            Console.WriteLine($"Generating token for user: {user.Email}, IsOrganizer: {user.IsOrganizer}");
            var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("IsOrganizer", user.IsOrganizer.ToString().ToLower())
            };
            if (user.IsOrganizer)
            {
                claims.Add(new Claim("scope", "organizer_access"));
                Console.WriteLine($"Added organizer_access scope for user: {user.Email}");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
