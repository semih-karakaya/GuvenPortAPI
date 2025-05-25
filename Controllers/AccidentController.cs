using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;
using Microsoft.AspNetCore.Authorization;

namespace GuvenPortAPI.Controllers
{
    [Route("api/accident")]
    [ApiController]
    [Authorize]
    public class AccidentController : ControllerBase
    {
        private readonly IAccidentService _accidentService;

        public AccidentController(IAccidentService accidentService)
        {
            _accidentService = accidentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AccidentCreateViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Geçersiz veri.");
            }

            // Modelden kazayı oluştur
            var accident = new Accident
            {
                AccDate = model.AccDate, // Nullable DateOnly
                AccTime = model.AccTime, // Nullable TimeOnly
                Fatality = model.Fatality ?? false, // Nullable bool, varsayılan false
                Injury = model.Injury ?? false, // Nullable bool, varsayılan false
                PropertyDamage = model.PropertyDamage ?? false, // Nullable bool, varsayılan false
                NearMiss = model.NearMiss ?? false, // Nullable bool, varsayılan false
                StoryOfAccident = model.StoryOfAccident,
                IdWorkplace = model.IdWorkplace ?? 0, // Nullable int, varsayılan 0
                SgkInfoDate = model.SgkInfoDate, // Nullable DateOnly
                SgkInfoCheck = model.SgkInfoCheck ?? false // Nullable bool, varsayılan false
            };

           
            var createdAccident = await _accidentService.AddAccidentAsync(accident);

           
            foreach (var staffId in model.SelectedStaffIds)
            {
                await _accidentService.AddStaffToAccidentAsync(createdAccident.Id, staffId);
            }

            foreach (var contractId in model.SelectedContractIds)
            {
                await _accidentService.AddContractToAccidentAsync(createdAccident.Id, contractId);
            }

            return CreatedAtAction(nameof(Create), new { id = createdAccident.Id }, createdAccident);
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
        [HttpPost("{accidentId}/add-staff/{staffId}")]
        public async Task<IActionResult> AddStaffToAccident(int accidentId, int staffId)
        {
            var result = await _accidentService.AddStaffToAccidentAsync(accidentId, staffId);
            if (!result) return BadRequest("Staff already added or invalid IDs.");
            return Ok(result);
        }

        [HttpDelete("{accidentId}/remove-staff/{staffId}")]
        public async Task<IActionResult> RemoveStaffFromAccident(int accidentId, int staffId)
        {
            var result = await _accidentService.RemoveStaffFromAccidentAsync(accidentId, staffId);
            if (!result) return NotFound("Staff not found in this accident.");
            return Ok(result);
        }

        [HttpGet("{accidentId}/staff")]
        public async Task<IActionResult> GetStaffByAccident(int accidentId)
        {
            var result = await _accidentService.GetStaffByAccidentAsync(accidentId);
            return Ok(result);
        }

        [HttpGet("staff/{staffId}/accidents")]
        public async Task<IActionResult> GetAccidentsByStaff(int staffId)
        {
            var result = await _accidentService.GetAccidentsByStaffAsync(staffId);
            return Ok(result);
        }
        [HttpPost("{accidentId}/add-contract/{contractId}")]
        public async Task<IActionResult> AddContractToAccident(int accidentId, int contractId)
        {
            var result = await _accidentService.AddContractToAccidentAsync(accidentId, contractId);
            if (!result) return BadRequest("Contract already added or invalid IDs.");
            return Ok(result);
        }

        [HttpDelete("{accidentId}/remove-contract/{contractId}")]
        public async Task<IActionResult> RemoveContractFromAccident(int accidentId, int contractId)
        {
            var result = await _accidentService.RemoveContractFromAccidentAsync(accidentId, contractId);
            if (!result) return NotFound("Contract not found in this accident.");
            return Ok(result);
        }

        [HttpGet("{accidentId}/contracts")]
        public async Task<IActionResult> GetContractsByAccident(int accidentId)
        {
            var result = await _accidentService.GetContractsByAccidentAsync(accidentId);
            return Ok(result);
        }

        [HttpGet("contract/{contractId}/accidents")]
        public async Task<IActionResult> GetAccidentsByContract(int contractId)
        {
            var result = await _accidentService.GetAccidentsByContractAsync(contractId);
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
