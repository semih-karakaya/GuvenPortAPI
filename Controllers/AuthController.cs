using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GuvenPortAPI.Models; // Staff ve diğer modellerin olduğu namespace
using GuvenPortAPI.Models.Interface; // IStaffService'in olduğu namespace
using System.Linq;
using Microsoft.AspNetCore.Authorization; // [AllowAnonymous] için

namespace GuvenPortAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Authorize]
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
        [AllowAnonymous] // Bu satırı ekle, login endpoint'i kimlik doğrulaması gerektirmez.
        public async Task<IActionResult> Login([FromBody] StaffLoginRequest request)
        {
            
            var staffList = await _staffService.GetAllStaffAsync(); 
            var staff = staffList.FirstOrDefault(x => x.Mail == request.mail && x.Password == request.password);


            if (staff == null)
            {
                
                return Unauthorized("Geçersiz kimlik bilgileri.");
            }


            
            bool isDoctor = staff.Doctor != null;

           
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, staff.Id.ToString()), 
                new Claim(ClaimTypes.Name, staff.Mail), 
                new Claim(ClaimTypes.Email, staff.Mail ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            };

           
            claims.Add(new Claim(ClaimTypes.Role, isDoctor ? "Doctor" : "Staff"));
            claims.Add(new Claim("is_doctor", isDoctor.ToString().ToLower()));

            // JWT Key ve Expiration alma
            var utcNow = DateTime.UtcNow; // UTC kullanmak iyi bir pratiktir
            var expires = utcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60")); 

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: utcNow, 
                expires: expires,
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, 
                Secure = true,   
                Expires = expires.ToLocalTime(), 
                SameSite = SameSiteMode.None, 
                Domain = ".localhost" 
            };

            Response.Cookies.Append("authToken", tokenString, cookieOptions); 
            return Ok(new
            {
                Token = tokenString,
                staff.Name,
                Role = isDoctor ? "Doctor" : "Staff"
            });
        }
    }
}
