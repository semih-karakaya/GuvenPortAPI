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
            // Kullanıcıyı Staff tablosunda bul (IStaffService üzerinden)
            // Not: Personel şifrelerinin hashlenmesi önemlidir. Eğer _staffService direkt DB'den düz metin alıyorsa güvenlik riskidir.
            // Örneğin burada BCrypt.Net.BCrypt.Verify(request.password, staff.Password) gibi bir kontrol olmalıydı eğer şifreler hashlenmişse.
            var staffList = await _staffService.GetAllStaffAsync(); // Tüm staff'ı çekmek yerine direkt FirstOrDefaultAsync ile sorgulasan daha performanslı olurdu
            var staff = staffList.FirstOrDefault(x => x.Mail == request.mail && x.Password == request.password);


            if (staff == null)
            {
                // Güvenlik için daha genel bir mesaj döndürmek daha iyidir
                return Unauthorized("Geçersiz kimlik bilgileri.");
            }


            // Doktor mu kontrolü (staff.Doctor navigasyon property'si null değilse doktor demektir)
            // Navigasyon property'nin yüklenmesi için IStaffService.GetAllStaffAsync() veya ilgili GetStaffByMailAsync() metodunun `.Include(s => s.Doctor)` yapması gerekebilir.
            bool isDoctor = staff.Doctor != null;

            // JWT Token için Claim'leri oluştur
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, staff.Id.ToString()), // Kullanıcının DB ID'si
                new Claim(ClaimTypes.Name, staff.Mail), // Kullanıcı adı olarak Mail kullanmak genellikle daha tutarlıdır
                new Claim(ClaimTypes.Email, staff.Mail ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT ID
            };

            // Rolü Claim olarak ekle (ClaimTypes.Role, ASP.NET Core'un beklediği standart ROLLER için kullanılan Claim Type'dır)
            claims.Add(new Claim(ClaimTypes.Role, isDoctor ? "Doctor" : "Staff"));
            claims.Add(new Claim("is_doctor", isDoctor.ToString().ToLower()));

            // JWT Key ve Expiration alma
            var utcNow = DateTime.UtcNow; // UTC kullanmak iyi bir pratiktir
            var expires = utcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60")); // Jwt:ExpireMinutes yoksa 60 dk varsayılan

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: utcNow, // Token'ın geçerli olacağı zaman (hemen geçerli)
                expires: expires,
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // JavaScript erişemez
                Secure = true,   // Sadece HTTPS üzerinden gönderilir (ÖNEMLİ!)
                Expires = expires.ToLocalTime(), // Token'ın süresiyle eşleştirmelisin
                SameSite = SameSiteMode.None, // Çapraz kaynak istekleri için (ÖNEMLİ!)
                Domain = ".localhost" 
            };

            Response.Cookies.Append("authToken", tokenString, cookieOptions); // Cookie adı 'authToken'
            return Ok(new
            {
                Token = tokenString,
                staff.Name,
                Role = isDoctor ? "Doctor" : "Staff"
            });
        }
    }
}
