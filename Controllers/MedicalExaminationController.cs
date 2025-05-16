using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Controllers
{
    [Route("api/medicalexamination")]
    [ApiController]
    public class MedicalExaminationController : ControllerBase
    {
        private readonly IMedicalExaminationService _medicalExaminationService;

        public MedicalExaminationController(IMedicalExaminationService medicalExaminationService)
        {
            _medicalExaminationService = medicalExaminationService;
        }

        [HttpPost("AddMedicalExamination")]
        public async Task<ActionResult<MedicalExamination>> AddMedicalExamination([FromBody] MedicalExamination examination)
        {
            var result = await _medicalExaminationService.AddMedicalExaminationAsync(examination);
            if (result == null) return BadRequest("Failed to add medical examination.");
            return Ok(result);
        }

        [HttpGet("ListMedicalExaminations")]
        public async Task<ActionResult<List<MedicalExamination>>> ListMedicalExaminations()
        {
            var result = await _medicalExaminationService.GetAllMedicalExaminationsAsync();
            if (result == null || result.Count == 0) return NotFound("No medical examinations found.");
            return Ok(result);
        }

        [HttpGet("GetMedicalExamination/{id}")]
        public async Task<ActionResult<MedicalExamination>> GetMedicalExamination(int id)
        {
            var result = await _medicalExaminationService.GetMedicalExaminationByIdAsync(id);
            if (result == null) return NotFound($"Medical examination with ID {id} not found.");
            return Ok(result);
        }

        [HttpPut("EditMedicalExamination")]
        public async Task<ActionResult<MedicalExamination>> EditMedicalExamination([FromBody] MedicalExamination examination)
        {
            var result = await _medicalExaminationService.UpdateMedicalExaminationAsync(examination);
            if (result == null) return NotFound("Failed to update medical examination.");
            return Ok(result);
        }

        [HttpDelete("DeleteMedicalExamination/{id}")]
        public async Task<ActionResult<bool>> DeleteMedicalExamination(int id)
        {
            var result = await _medicalExaminationService.DeleteMedicalExaminationAsync(id);
            if (!result) return NotFound($"Medical examination with ID {id} not found.");
            return Ok(result);
        }
    }
}
