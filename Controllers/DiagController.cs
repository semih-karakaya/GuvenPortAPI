using GuvenPortAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;   // GetDbConnection için
using System.Linq;

namespace GuvenPortAPI.Controllers
{
    [ApiController]
    [Route("api/diag")]
    public class DiagnosticsController : ControllerBase
    {
        // 1) Burada DbContext'i tanımlayın:
        private readonly isgportalContext _ctx;

        // 2) Constructor üzerinden inject edin:
        public DiagnosticsController(isgportalContext ctx)
        {
            _ctx = ctx;
        }

        // 3) Test endpoint'i:
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            // 4) Aktif DB bağlantısını alın ve current_user'ı sorgulatın
            var conn = _ctx.Database.GetDbConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT current_user;";
            var dbUser = cmd.ExecuteScalar()?.ToString();

            // 5) Claim'den is_doctor'ı okuyun
            bool claimIsDoctor = User.Claims.Any(c => c.Type == "is_doctor" && c.Value == "true");

            // 6) Response dönün
            return Ok(new
            {
                ClaimIsDoctor = claimIsDoctor,
                UsedConnection = conn.ConnectionString,
                DbUser = dbUser
            });
        }
    }
}
