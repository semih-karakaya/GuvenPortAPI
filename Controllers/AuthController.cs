using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;
using System.Linq;

namespace GuvenPortAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly IConfiguration _configuration;

        public AuthController(IStaffService staffService, IConfiguration configuration)
        {
            _staffService = staffService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] StaffLoginRequest request)
        {
            // Kullanıcıyı Staff tablosunda bul
            var staffList = await _staffService.GetAllStaffAsync();
            var staff = staffList.FirstOrDefault(x => x.Mail == request.mail && x.Password == request.password);

            if (staff == null)
                return Unauthorized("Invalid credentials.");

            // Doktor mu kontrolü
            bool isDoctor = staff.Doctor != null;

            // JWT Token oluştur
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, staff.Id.ToString()),
        new Claim(ClaimTypes.Name, staff.Name ?? ""),
        new Claim(ClaimTypes.Email, staff.Mail ?? "")
    };

            // İsteğe bağlı: Rolü claim olarak ekle
            claims.Add(new Claim(ClaimTypes.Role, isDoctor ? "Doctor" : "Staff"));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                isDoctor, // İstersen cevaba da ekleyebilirsin
                Name = staff.Name
            });
        }

    }
}

