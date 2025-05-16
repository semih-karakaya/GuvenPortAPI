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
    }
    [Route("api/workplace")]
    [ApiController]
    public class WorkplaceController : Controller
    {
        private readonly IWorkplaceService _workplaceService;

        public WorkplaceController(IWorkplaceService workplaceService)
        {
            _workplaceService = workplaceService;
        }

        /// <summary>
        /// Adds a new workplace to the database.
        /// </summary>
        /// <param name="workplace">The workplace object to add.</param>
        /// <returns>The added workplace object or null if the operation fails.</returns>
        [Route("AddWorkplace")]
        [HttpPost]
        public async Task<ActionResult<Workplace>> AddWorkplaceToDatabase(Workplace workplace)
        {
            var result = await _workplaceService.AddWorkplaceService(workplace);

            if (result == null)
            {
                return BadRequest("Failed to add workplace.");
            }

            return Ok(result);
        }
    [HttpGet("{workplaceId}/medical-status")]
    public async Task<IActionResult> GetWorkplaceMedicalStatus(int officeid)
    {
        var result = await _workplaceService.GetOfficeEmployeesMedicalStatusAsync(officeid);
        if (result == null)
            return NotFound($"Workplace with ID {officeid} not found.");
        return Ok(result);
    }
    [HttpGet("doctor/{doctorId}/medical-examinations")] 
    public async Task<IActionResult> GetMedicalExaminationsByDoctorId(int doctorId) { 
        var result = await _workplaceService.GetMedicalExaminationsByDoctorIdAsync(doctorId); 
        return Ok(result); 
    }
    [HttpGet("employee/{employeeId}/medical-examinations")]
    public async Task<IActionResult> GetMedicalExaminationsByEmployeeId(int employeeId)
    {
        var result = await _workplaceService.GetMedicalExaminationsByEmployeeIdAsync(employeeId);
        return Ok(result);
    }

    [HttpGet("employee/{employeeId}/medical-examinations/count")]
    public async Task<IActionResult> GetMedicalExaminationCountByEmployeeId(int employeeId)
    {
        var count = await _workplaceService.GetMedicalExaminationCountByEmployeeIdAsync(employeeId);
        return Ok(new { examination_count = count });
    }
    [HttpGet("workplace-exam-counts")]
    public async Task<IActionResult> GetWorkplaceExamCounts()
    {
        var result = await _workplaceService.GetWorkplaceExamCountsAsync();
        return Ok(result);
    }

    [HttpGet("doctor/{doctorId}/medical-examination-count")] 
    public async Task<IActionResult> GetMedicalExaminationCountByDoctorId(int doctorId) { 
        var count = await _workplaceService.GetMedicalExaminationCountByDoctorIdAsync(doctorId); 
        return Ok(new { DoctorId = doctorId, ExaminationCount = count }); 
    }

    [HttpGet("{workplaceId}/accident-count")]
    public async Task<IActionResult> GetEmployeeAccidentCountDtoAsync(
    int workplaceId,
    [FromQuery] DateOnly startDate,
    [FromQuery] DateOnly endDate)
    {
        var result = await _workplaceService.GetWorkplaceAccidentCountAsync(workplaceId, startDate, endDate);
        return Ok(result);
    }
    [HttpGet("employee/{employeeId}/accident-count-dto")]
    public async Task<IActionResult> GetEmployeeAccidentCountDto(int employeeId)
    {
        var result = await _workplaceService.GetEmployeeAccidentCountDtoAsync(employeeId);
        return Ok(result);
    }
    /// <summary>
    /// Lists all active workplaces.
    /// </summary>
    /// <returns>A list of workplaces.</returns>
    [Route("ListWorkplaces")]
        [HttpGet]
        public async Task<ActionResult<List<Workplace>>> ListWorkplacesFromDatabase()
        {
            var result = await _workplaceService.ListWorkplaceService();

            if (result == null || result.Count == 0)
            {
                return NotFound("No workplaces found.");
            }

            return Ok(result);
        }
    [HttpGet("employees-without-entry-exam")]
    public async Task<IActionResult> GetEmployeesWithoutEntryExam()
    {
        var result = await _workplaceService.GetEmployeesWithoutEntryExamAsync();
        return Ok(result);
    }
    [HttpGet("employee-demographics")]
    public async Task<IActionResult> GetEmployeeDemographics()
    {
        var result = await _workplaceService.GetEmployeeDemographicsAsync();
        return Ok(result);
    }

    [HttpGet("active-contract-employee-demographics")]
    public async Task<IActionResult> GetActiveContractEmployeeDemographics()
    {
        var result = await _workplaceService.GetActiveContractEmployeeDemographicsAsync();
        return Ok(result);
    }


    /// <summary>
    /// Gets details for a single workplace by its ID.
    /// </summary>
    /// <param name="id">The ID of the workplace to retrieve.</param>
    /// <returns>A workplace object or null if not found.</returns>
    [HttpGet]
        [Route("GetWorkplaceWithID/{id}")]
        public async Task<ActionResult<Workplace>> GetWorkplaceDetails(int id)
        {
            var result = await _workplaceService.GetOneWorkplaceFromID(id);

            if (result == null)
            {
                return NotFound($"Workplace with ID {id} not found.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Edits an existing workplace's details.
        /// </summary>
        /// <param name="workplace">The workplace object with updated details.</param>
        /// <returns>The updated workplace object or null if the operation fails.</returns>
        [Route("EditWorkplace")]
        [HttpPut]
        public async Task<ActionResult<Workplace>> EditWorkplaceInDatabase(Workplace workplace)
        {
            var updatedWorkplace = await _workplaceService.EditWorkplaceService(workplace);

            if (updatedWorkplace == null)
            {
                return NotFound($"Workplace with ID {workplace.Id} not found or failed to update.");
            }

            return Ok(updatedWorkplace);
        }

        /// <summary>
        /// Deletes (marks as inactive) a workplace by its ID.
        /// </summary>
        /// <param name="id">The ID of the workplace to delete.</param>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
        [HttpDelete]
        [Route("DeleteWorkplace/{id}")]
        public async Task<ActionResult<bool>> DeleteWorkplace(int id)
        {
            var result = await _workplaceService.DeleteWorkplaceService(id);

            if (!result)
            {
                return NotFound($"Workplace with ID {id} not found.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Adds a staff member to a workplace.
        /// </summary>
        /// <param name="workplaceId">The ID of the workplace.</param>
        /// <param name="staffId">The ID of the staff member.</param>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        [HttpPost("AddStaffToWorkplace/{workplaceId}/{staffId}")]
        public async Task<ActionResult<bool>> AddStaffToWorkplace(int workplaceId, int staffId)
        {
            var result = await _workplaceService.AddStaffToWorkplace(workplaceId, staffId);

            if (!result)
            {
                return BadRequest("Failed to add staff to workplace.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Removes a staff member from a workplace.
        /// </summary>
        /// <param name="workplaceId">The ID of the workplace.</param>
        /// <param name="staffId">The ID of the staff member.</param>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        [HttpDelete("RemoveStaffFromWorkplace/{workplaceId}/{staffId}")]
        public async Task<ActionResult<bool>> RemoveStaffFromWorkplace(int workplaceId, int staffId)
        {
            var result = await _workplaceService.RemoveStaffFromWorkplace(workplaceId, staffId);

            if (!result)
            {
                return NotFound("Staff not found in this workplace.");
            }

            return Ok(result);
        }
    }

