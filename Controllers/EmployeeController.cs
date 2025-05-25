using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.DTOs;
using GuvenPortAPI.Models.Interface;
using Microsoft.AspNetCore.Authorization;

namespace GuvenPortAPI.Controllers
{
    [Route("api/employees")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost("add")]
        public async Task<ActionResult<EmployeeDto>> AddEmployee([FromBody] EmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _employeeService.AddEmployeeAsync(employeeDto);
            if (result == null) return BadRequest("Failed to add employee.");

            return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Id }, result);
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<Employee>>> ListEmployees()
        {
            var result = await _employeeService.GetAllEmployeesAsync();
            if (result == null || result.Count == 0) return NotFound("No employees found.");

            return Ok(result);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);
            if (result == null) return NotFound($"Employee with ID {id} not found.");
            return Ok(result);
        }

        [HttpPut("edit")]
        public async Task<ActionResult<EmployeeDto>> EditEmployee([FromBody] EmployeeDto employee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _employeeService.UpdateEmployeeAsync(employee);
            if (result == null) return NotFound("Failed to update employee.");

            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<bool>> DeleteEmployee(int id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (!result) return NotFound($"Employee with ID {id} not found.");

            return Ok(result);
        }
    }
}
