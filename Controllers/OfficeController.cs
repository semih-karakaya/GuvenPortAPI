// using GuvenPortAPI.Models; // Make sure your models namespace is included here
// using GuvenPortAPI.Service; // Make sure your service namespace is included here
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using Newtonsoft.Json; // Keep if you use JsonConvert
using Microsoft.AspNetCore.Authorization; // For the Authorize attribute
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;

// Adjust the namespace to match your project structure
namespace GuvenPortAPI.Controllers
{
    //[Authorize] // Apply authorization if needed
    [Route("api/office")] // Define the base route for this controller
    [ApiController]
    public class OfficeController : Controller
    {
        // Use the interface if you create one: private readonly IOfficeService _officeService;
        private readonly IOfficeService _officeService; // Using the concrete class for now

        public OfficeController(IOfficeService officeService) // Inject the OfficeService
        {
            _officeService = officeService;
        }

        /// <summary>
        /// Adds a new office to the database.
        /// </summary>
        /// <param name="office">The office object to add.</param>
        /// <returns>The added office object or null if the operation fails.</returns>
        [Route("AddOffice")]
        [HttpPost]
        public async Task<ActionResult<Office>> AddOfficeToDatabase(Office office)
        {
            var result = await _officeService.AddOfficeService(office);

            if (result == null)
            {
                // Return a BadRequest or other appropriate status code if adding fails
                return BadRequest("Failed to add office.");
            }

            return Ok(result); // Return 200 Ok with the added office
        }

        /// <summary>
        /// Lists all active offices.
        /// </summary>
        /// <returns>A list of vmOfficeDetails objects representing the offices.</returns>
        [Route("ListOfficesFunction")]
        [HttpGet]
        public async Task<ActionResult<List<vmOfficeDetails>>> ListOfficeFromDatabase()
        {
            var result = await _officeService.ListOfficeService();

            if (result == null || result.Count == 0)
            {
                // Return NotFound if no offices are found
                return NotFound("No offices found.");
            }

            return Ok(result); // Return 200 Ok with the list of offices
        }

        /// <summary>
        /// Gets details for a single office by its ID.
        /// </summary>
        /// <param name="id">The ID of the office to retrieve.</param>
        /// <returns>A vmOfficeDetails object representing the office or null if not found.</returns>
        [HttpGet]
        [Route("GetOfficeWithID/{id}")] // Use route parameter for ID
        public async Task<ActionResult<vmOfficeDetails>> GetOfficeDetails(int id)
        {
            var result = await _officeService.GetOneOfficeFromID(id);

            if (result == null)
            {
                // Return NotFound if the office is not found
                return NotFound($"Office with ID {id} not found.");
            }

            return Ok(result); // Return 200 Ok with the office details
        }

        /// <summary>
        /// Edits an existing office's details.
        /// </summary>
        /// <param name="office">The office object with updated details.</param>
        /// <returns>The updated office object or null if the operation fails.</returns>
        [Route("EditOffice")]
        [HttpPut]
        public async Task<ActionResult<Office>> EditOfficeInDatabase(Office office)
        {
            var updatedOffice = await _officeService.EditOfficeService(office);

            if (updatedOffice == null)
            {
                // Return NotFound if the office to edit is not found, or BadRequest if other issues
                return NotFound($"Office with ID {office.Id} not found or failed to update.");
            }

            return Ok(updatedOffice); // Return 200 Ok with the updated office
        }

        /// <summary>
        /// Deletes (marks as inactive) an office by its ID.
        /// </summary>
        /// <param name="id">The ID of the office to delete.</param>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
        [HttpDelete] // Use HttpDelete for deletion
        [Route("DeleteOffice/{id}")] // Use route parameter for ID
        public async Task<ActionResult<bool>> DeleteOffice(int id)
        {
            var result = await _officeService.DeleteOfficeService(id);

            if (!result)
            {
                // Return NotFound if the office is not found
                return NotFound($"Office with ID {id} not found.");
            }

            return Ok(result); // Return 200 Ok with the result (true)
        }

        // You can add more endpoints here for managing StaffOffice and Workplace relationships
        // For example:
        // [HttpPost("AddStaffToOffice/{officeId}/{staffId}")]
        // public async Task<ActionResult<bool>> AddStaffToOffice(int officeId, int staffId)
        // {
        //     var result = await _officeService.AddStaffToOffice(officeId, staffId);
        //     if (!result) return BadRequest("Failed to add staff to office.");
        //     return Ok(result);
        // }

        // [HttpDelete("RemoveStaffFromOffice/{officeId}/{staffId}")]
        // public async Task<ActionResult<bool>> RemoveStaffFromOffice(int officeId, int staffId)
        // {
        //     var result = await _officeService.RemoveStaffFromOffice(officeId, staffId);
        //     if (!result) return NotFound("Staff not found in this office.");
        //     return Ok(result);
        // }

        // Add similar endpoints for Workplaces
    }
}
