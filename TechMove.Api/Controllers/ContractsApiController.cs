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
    }
}