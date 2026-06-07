using Microsoft.AspNetCore.Mvc;
using TechMove.Api.Repositories;

namespace TechMove.Api.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsApiController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsApiController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            var clients = await _clientRepository.GetAllAsync();

            var result = clients.Select(c => new
            {
                c.Id,
                c.Name,
                c.ContactDetails,
                c.Region
            });

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
            {
                return NotFound(new { message = "Client not found." });
            }

            return Ok(new
            {
                client.Id,
                client.Name,
                client.ContactDetails,
                client.Region
            });
        }
    }
}