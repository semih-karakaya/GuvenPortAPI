using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost("AddEmployee")]
        public async Task<ActionResult<Employee>> AddEmployee([FromBody] Employee employee)
        {
            var result = await _employeeService.AddEmployeeAsync(employee);
            if (result == null) return BadRequest("Failed to add employee.");
            return Ok(result);
        }

        [HttpGet("ListEmployees")]
        public async Task<ActionResult<List<Employee>>> ListEmployees()
        {
            var result = await _employeeService.GetAllEmployeesAsync();
            if (result == null || result.Count == 0) return NotFound("No employees found.");
            return Ok(result);
        }

        [HttpGet("GetEmployee/{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);
            if (result == null) return NotFound($"Employee with ID {id} not found.");
            return Ok(result);
        }

        [HttpPut("EditEmployee")]
        public async Task<ActionResult<Employee>> EditEmployee([FromBody] Employee employee)
        {
            var result = await _employeeService.UpdateEmployeeAsync(employee);
            if (result == null) return NotFound("Failed to update employee.");
            return Ok(result);
        }

        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<ActionResult<bool>> DeleteEmployee(int id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (!result) return NotFound($"Employee with ID {id} not found.");
            return Ok(result);
        }
    }
}
