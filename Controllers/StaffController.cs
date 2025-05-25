using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;
using Microsoft.AspNetCore.Authorization;


namespace GuvenPortAPI.Controllers
{
    [Route("api/staff")]
    [ApiController]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpPost("AddStaff")]
        public async Task<ActionResult<Staff>> AddStaff([FromBody] Staff staff)
        {
            var result = await _staffService.AddStaffAsync(staff);
            if (result == null) return BadRequest("Failed to add staff.");
            return Ok(result);
        }

        [HttpGet("ListStaff")]
        public async Task<ActionResult<List<Staff>>> ListStaff()
        {
            var result = await _staffService.GetAllStaffAsync();
            if (result == null || result.Count == 0) return NotFound("No staff found.");
            return Ok(result);
        }

        [HttpGet("GetStaff/{id}")]
        public async Task<ActionResult<Staff>> GetStaff(int id)
        {
            var result = await _staffService.GetStaffByIdAsync(id);
            if (result == null) return NotFound($"Staff with ID {id} not found.");
            return Ok(result);
        }

        [HttpPut("EditStaff")]
        public async Task<ActionResult<Staff>> EditStaff([FromBody] Staff staff)
        {
            var result = await _staffService.UpdateStaffAsync(staff);
            if (result == null) return NotFound("Failed to update staff.");
            return Ok(result);
        }

        [HttpDelete("DeleteStaff/{id}")]
        public async Task<ActionResult<bool>> DeleteStaff(int id)
        {
            var result = await _staffService.DeleteStaffAsync(id);
            if (!result) return NotFound($"Staff with ID {id} not found.");
            return Ok(result);
        }
    }
}
