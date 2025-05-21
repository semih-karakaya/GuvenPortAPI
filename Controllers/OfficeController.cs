using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuvenPortAPI.Controllers
{
    [Route("api/office")]
    [ApiController]
    public class OfficeController : ControllerBase
    {
        private readonly IOfficeService _officeService;

        public OfficeController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        [HttpPost("add")]
        public async Task<ActionResult<Office>> AddOfficeToDatabase([FromBody] Office office)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _officeService.AddOfficeService(office);

            if (result == null)
            {
                return BadRequest("Failed to add office.");
            }

            return Ok(result);
        }

        [HttpGet("total-active-companies")]
        public async Task<IActionResult> GetTotalActiveCompanies()
        {
            var total = await _officeService.GetTotalActiveCompaniesAsync();
            return Ok(new { total_active_companies = total });
        }
        [HttpGet("{officeId}/active-workplaces")]
        public async Task<IActionResult> GetActiveWorkplacesByOfficeId(int officeId)
        {
            // Input validation for officeId can be added
            var workplaces = await _officeService.GetActiveWorkplacesByOfficeIdAsync(officeId);

            if (workplaces == null || !workplaces.Any())
            {
                return NoContent(); // Return No Content if no workplaces found
            }

            return Ok(workplaces);
        }


        [HttpGet("total-active-workplaces")]
        public async Task<IActionResult> GetTotalActiveWorkplaces()
        {
            var total = await _officeService.GetTotalActiveWorkplacesAsync();
            return Ok(new { total_active_workplaces = total });
        }

        // Tüm ofisleri listele
        [HttpGet("all")]
        public async Task<ActionResult<List<vmOfficeDetails>>> ListOffices()
        {
            var result = await _officeService.ListOfficeService();

            if (result == null || result.Count == 0)
            {
                return NotFound("No offices found.");
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<vmOfficeDetails>> GetOfficeDetails(int id)
        {
            var result = await _officeService.GetOneOfficeFromID(id);

            if (result == null)
            {
                return NotFound($"Office with ID {id} not found.");
            }

            return Ok(result);
        }

        [HttpPut("edit")]
        public async Task<ActionResult<Office>> EditOfficeInDatabase([FromBody] Office office)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedOffice = await _officeService.EditOfficeService(office);

            if (updatedOffice == null)
            {
                return NotFound($"Office with ID {office.Id} not found or failed to update.");
            }

            return Ok(updatedOffice);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteOffice(int id)
        {
            var result = await _officeService.DeleteOfficeService(id);

            if (!result)
            {
                return NotFound($"Office with ID {id} not found.");
            }

            return Ok(result);
        }

        [HttpGet("active-with-manager")]
        public async Task<IActionResult> GetActiveOfficesWithManager()
        {
            var offices = await _officeService.GetActiveOfficesWithManagerAsync();
            return Ok(offices);
        }
    }
}
