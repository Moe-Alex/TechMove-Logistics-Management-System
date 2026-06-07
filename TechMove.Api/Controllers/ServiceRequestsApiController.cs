using Microsoft.AspNetCore.Mvc;
using TechMove.Api.DTOs;
using TechMove.Api.Services;

namespace TechMove.Api.Controllers
{
    [ApiController]
    [Route("api/service-requests")]
    public class ServiceRequestsApiController : ControllerBase
    {
        private readonly IServiceRequestApiService _serviceRequestApiService;

        public ServiceRequestsApiController(IServiceRequestApiService serviceRequestApiService)
        {
            _serviceRequestApiService = serviceRequestApiService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ServiceRequestDto>>> GetServiceRequests()
        {
            var serviceRequests = await _serviceRequestApiService.GetAllAsync();

            return Ok(serviceRequests);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceRequestDto>> GetServiceRequestById(int id)
        {
            var serviceRequest = await _serviceRequestApiService.GetByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound(new { message = "Service request not found." });
            }

            return Ok(serviceRequest);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequestDto>> CreateServiceRequest(CreateServiceRequestDto dto)
        {
            try
            {
                var createdServiceRequest = await _serviceRequestApiService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetServiceRequestById),
                    new { id = createdServiceRequest.Id },
                    createdServiceRequest);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateServiceRequest(int id, UpdateServiceRequestDto dto)
        {
            try
            {
                var updated = await _serviceRequestApiService.UpdateAsync(id, dto);

                if (!updated)
                {
                    return NotFound(new { message = "Service request not found." });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var deleted = await _serviceRequestApiService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Service request not found." });
            }

            return NoContent();
        }
    }
}