using Microsoft.AspNetCore.Mvc;
using TechMove.Api.DTOs;
using TechMove.Api.Services;

namespace TechMove.Api.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsApiController : ControllerBase
    {
        private readonly IClientApiService _clientApiService;

        public ClientsApiController(IClientApiService clientApiService)
        {
            _clientApiService = clientApiService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ClientDto>>> GetClients()
        {
            var clients = await _clientApiService.GetAllAsync();

            return Ok(clients);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClientDto>> GetClientById(int id)
        {
            var client = await _clientApiService.GetByIdAsync(id);

            if (client == null)
            {
                return NotFound(new { message = "Client not found." });
            }

            return Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<ClientDto>> CreateClient(CreateClientDto dto)
        {
            var createdClient = await _clientApiService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetClientById),
                new { id = createdClient.Id },
                createdClient);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateClient(int id, UpdateClientDto dto)
        {
            var updated = await _clientApiService.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound(new { message = "Client not found." });
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var deleted = await _clientApiService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Client not found." });
            }

            return NoContent();
        }
    }
}