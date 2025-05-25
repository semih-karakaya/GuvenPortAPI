using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;
using Microsoft.AspNetCore.Authorization;

namespace GuvenPortAPI.Controllers
{
    [Route("api/contracts")]
    [ApiController]
    [Authorize]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpPost("add")]
        public async Task<ActionResult<Contract>> AddContract([FromBody] Contract contract)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _contractService.AddContractAsync(contract);
            if (result == null) return BadRequest("Failed to add contract.");

            return CreatedAtAction(nameof(GetContractById), new { id = result.Id }, result);
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<Contract>>> ListContracts()
        {
            var result = await _contractService.GetAllContractsAsync();
            if (result == null || result.Count == 0) return NotFound("No contracts found.");

            return Ok(result);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Contract>> GetContractById(int id)
        {
            var result = await _contractService.GetContractByIdAsync(id);
            if (result == null) return NotFound($"Contract with ID {id} not found.");
            return Ok(result);
        }

        [HttpPut("edit")]
        public async Task<ActionResult<Contract>> EditContract([FromBody] Contract contract)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _contractService.UpdateContractAsync(contract);
            if (result == null) return NotFound("Failed to update contract.");

            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<bool>> DeleteContract(int id)
        {
            var result = await _contractService.DeleteContractAsync(id);
            if (!result) return NotFound($"Contract with ID {id} not found.");

            return Ok(result);
        }

        [HttpGet("byworkplace/{workplaceId}/employees")]
        public async Task<IActionResult> GetEmployeesByWorkplaceId(int workplaceId)
        {
            var contractsWithEmployees = await _contractService.GetActiveContractsWithEmployeesByWorkplaceIdAsync(workplaceId);

            if (!contractsWithEmployees.Any())
            {
                return NotFound(); // Eğer aktif kontratlar yoksa 404 döndür
            }

            return Ok(contractsWithEmployees); // Aktif kontratlar ve çalışanları döner
        }
        [HttpGet("getnames/{cid}")]
        public async Task<IActionResult> getnames(int cid)
        {
            var contractsWithEmployees = await _contractService.getnameswithcontractid(cid);

            if (!contractsWithEmployees.Any())
            {
                return NotFound(); // Eğer aktif kontratlar yoksa 404 döndür
            }

            return Ok(contractsWithEmployees); // Aktif kontratlar ve çalışanları döner
        }
    }
}
