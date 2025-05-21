using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuvenPortAPI.Controllers
{
    [Route("api/workplace")]
    [ApiController]
    public class WorkplaceController : ControllerBase
    {
        private readonly IWorkplaceService _workplaceService;

        public WorkplaceController(IWorkplaceService workplaceService)
        {
            _workplaceService = workplaceService;
        }

        [HttpPost("add")]
        public async Task<ActionResult<Workplace>> AddWorkplace([FromBody] Workplace workplace)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _workplaceService.AddWorkplaceService(workplace);

            if (result == null)
            {
                return BadRequest("Failed to add workplace.");
            }

            return Ok(result);
        }
        [HttpGet("GetWorkplaceName/{id}")]
        public async Task<IActionResult> GetWorkplaceName(int id)
        {
            var name = await _workplaceService.GetWorkplaceNameByIdAsync(id);
            if (string.IsNullOrEmpty(name))
                return NotFound($"Workplace with ID {id} not found.");
            return Ok(name);
        }


        [HttpGet("{workplaceId}/medical-status")]
        public async Task<IActionResult> GetWorkplaceMedicalStatus(int workplaceId)
        {
            var result = await _workplaceService.GetOfficeEmployeesMedicalStatusAsync(workplaceId);
            if (result == null)
                return NotFound($"Workplace with ID {workplaceId} not found.");
            return Ok(result);
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<Workplace>>> ListWorkplaces()
        {
            var result = await _workplaceService.ListWorkplaceService();

            if (result == null || result.Count == 0)
            {
                return NotFound("No workplaces found.");
            }

            return Ok(result);
        }

        // Diğer metotlar buraya eklenebilir...
    }
}
