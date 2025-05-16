using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Controllers
{
    [Route("api/contract")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpPost("AddContract")]
        public async Task<ActionResult<Contract>> AddContract([FromBody] Contract contract)
        {
            var result = await _contractService.AddContractAsync(contract);
            if (result == null) return BadRequest("Failed to add contract.");
            return Ok(result);
        }

        [HttpGet("ListContracts")]
        public async Task<ActionResult<List<Contract>>> ListContracts()
        {
            var result = await _contractService.GetAllContractsAsync();
            if (result == null || result.Count == 0) return NotFound("No contracts found.");
            return Ok(result);
        }

        [HttpGet("GetContract/{id}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var result = await _contractService.GetContractByIdAsync(id);
            if (result == null) return NotFound($"Contract with ID {id} not found.");
            return Ok(result);
        }

        [HttpPut("EditContract")]
        public async Task<ActionResult<Contract>> EditContract([FromBody] Contract contract)
        {
            var result = await _contractService.UpdateContractAsync(contract);
            if (result == null) return NotFound("Failed to update contract.");
            return Ok(result);
        }

        [HttpDelete("DeleteContract/{id}")]
        public async Task<ActionResult<bool>> DeleteContract(int id)
        {
            var result = await _contractService.DeleteContractAsync(id);
            if (!result) return NotFound($"Contract with ID {id} not found.");
            return Ok(result);
        }
    }
}
