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
            // Kullan?c?y? Staff tablosunda bul
            var staffList = await _staffService.GetAllStaffAsync();
            var staff = staffList.FirstOrDefault(x => x.Mail == request.Mail && x.Password == request.Password);

            if (staff == null)
                return Unauthorized("Invalid credentials.");

            // JWT Token olu?tur
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, staff.Id.ToString()),
                new Claim(ClaimTypes.Name, staff.Name ?? ""),
                new Claim(ClaimTypes.Email, staff.Mail ?? "")
            };

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
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
