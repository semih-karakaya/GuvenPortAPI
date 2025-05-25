// File: Controllers/StaffExaminationSummaryController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;
using Microsoft.AspNetCore.Authorization;

namespace GuvenPortAPI.Controllers
{
    [Route("api/staffexaminationsummary")]
    [ApiController]
    [Authorize]
    public class StaffExaminationSummaryController : ControllerBase
    {
        private readonly IStaffExaminationSummaryService _service;

        public StaffExaminationSummaryController(IStaffExaminationSummaryService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<StaffExaminationSummary>>> List()
        {
            var result = await _service.GetAllAsync();
            if (result == null || result.Count == 0)
                return NotFound("No staff examination summaries found.");
            return Ok(result);
        }
    }
}
