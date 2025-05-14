using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("authapi/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly UserManager<Doctor> _userManager;

        public DoctorController(UserManager<Doctor> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("all")]
        public IActionResult GetAllDoctors()
        {
            var doctors = _userManager.Users.ToList();
            return Ok(doctors);
        }
    }
}
