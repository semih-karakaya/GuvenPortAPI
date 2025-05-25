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
       
        private readonly isgportalContext _ctx;


        public DiagnosticsController(isgportalContext ctx)
        {
            _ctx = ctx;
        }

        
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            
            var conn = _ctx.Database.GetDbConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT current_user;";
            var dbUser = cmd.ExecuteScalar()?.ToString();

          
            bool claimIsDoctor = User.Claims.Any(c => c.Type == "is_doctor" && c.Value == "true");

         
            return Ok(new
            {
                ClaimIsDoctor = claimIsDoctor,
                UsedConnection = conn.ConnectionString,
                DbUser = dbUser
            });
        }
    }
}
