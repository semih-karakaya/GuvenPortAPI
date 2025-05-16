using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Controllers
{
    [Route("api/accident")]
    [ApiController]
    public class AccidentController : ControllerBase
    {
        private readonly IAccidentService _accidentService;

        public AccidentController(IAccidentService accidentService)
        {
            _accidentService = accidentService;
        }

        [HttpPost("AddAccident")]
        public async Task<ActionResult<Accident>> AddAccident([FromBody] Accident accident)
        {
            var result = await _accidentService.AddAccidentAsync(accident);
            if (result == null) return BadRequest("Failed to add accident.");
            return Ok(result);
        }

        [HttpGet("ListAccidents")]
        public async Task<ActionResult<List<Accident>>> ListAccidents()
        {
            var result = await _accidentService.GetAllAccidentsAsync();
            if (result == null || result.Count == 0) return NotFound("No accidents found.");
            return Ok(result);
        }

        [HttpGet("GetAccident/{id}")]
        public async Task<ActionResult<Accident>> GetAccident(int id)
        {
            var result = await _accidentService.GetAccidentByIdAsync(id);
            if (result == null) return NotFound($"Accident with ID {id} not found.");
            return Ok(result);
        }

        [HttpPut("EditAccident")]
        public async Task<ActionResult<Accident>> EditAccident([FromBody] Accident accident)
        {
            var result = await _accidentService.UpdateAccidentAsync(accident);
            if (result == null) return NotFound("Failed to update accident.");
            return Ok(result);
        }

        [HttpDelete("DeleteAccident/{id}")]
        public async Task<ActionResult<bool>> DeleteAccident(int id)
        {
            var result = await _accidentService.DeleteAccidentAsync(id);
            if (!result) return NotFound($"Accident with ID {id} not found.");
            return Ok(result);
        }
    }
}
