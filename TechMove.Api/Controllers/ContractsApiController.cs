using Microsoft.AspNetCore.Mvc;
using TechMove.Api.DTOs;
using TechMove.Api.Services;
using TechMove.Models.Enums;

namespace TechMove.Api.Controllers
{
    [ApiController]
    [Route("api/contracts")]
    public class ContractsApiController : ControllerBase
    {
        private readonly IContractApiService _contractApiService;

        public ContractsApiController(IContractApiService contractApiService)
        {
            _contractApiService = contractApiService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ContractDto>>> GetContracts(
            [FromQuery] ContractStatus? status,
            [FromQuery] DateTime? startDateFrom,
            [FromQuery] DateTime? endDateTo)
        {
            var contracts = await _contractApiService.GetContractsAsync(status, startDateFrom, endDateTo);

            return Ok(contracts);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContractDto>> GetContractById(int id)
        {
            var contract = await _contractApiService.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound(new { message = "Contract not found." });
            }

            return Ok(contract);
        }

        [HttpPost]
        public async Task<ActionResult<ContractDto>> CreateContract(CreateContractDto dto)
        {
            try
            {
                var createdContract = await _contractApiService.CreateContractAsync(dto);

                return CreatedAtAction(
                    nameof(GetContractById),
                    new { id = createdContract.Id },
                    createdContract);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateContract(int id, UpdateContractDto dto)
        {
            try
            {
                var updated = await _contractApiService.UpdateContractAsync(id, dto);

                if (!updated)
                {
                    return NotFound(new { message = "Contract not found." });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateContractStatus(int id, UpdateContractStatusDto dto)
        {
            var updated = await _contractApiService.UpdateContractStatusAsync(id, dto);

            if (!updated)
            {
                return NotFound(new { message = "Contract not found." });
            }

            return NoContent();
        }

        [HttpPost("{id:int}/agreement")]
        public async Task<IActionResult> UploadAgreement(int id, IFormFile file)
        {
            try
            {
                var uploaded = await _contractApiService.UploadAgreementAsync(id, file);

                if (!uploaded)
                {
                    return NotFound(new { message = "Contract not found." });
                }

                return Ok(new { message = "Agreement uploaded successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}/agreement")]
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var result = await _contractApiService.DownloadAgreementAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "Agreement file not found." });
            }

            return File(result.Value.FileBytes, "application/pdf", result.Value.FileName);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            try
            {
                var deleted = await _contractApiService.DeleteContractAsync(id);

                if (!deleted)
                {
                    return NotFound(new { message = "Contract not found." });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}