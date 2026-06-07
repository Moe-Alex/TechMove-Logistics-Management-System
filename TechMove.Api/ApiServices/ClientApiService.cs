using TechMove.Api.DTOs;
using TechMove.Api.Repositories;
using TechMove.Models;

namespace TechMove.Api.Services
{
    public class ClientApiService : IClientApiService
    {
        private readonly IClientRepository _clientRepository;

        public ClientApiService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<List<ClientDto>> GetAllAsync()
        {
            var clients = await _clientRepository.GetAllAsync();

            return clients.Select(MapToDto).ToList();
        }

        public async Task<ClientDto?> GetByIdAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
            {
                return null;
            }

            return MapToDto(client);
        }

        public async Task<ClientDto> CreateAsync(CreateClientDto dto)
        {
            var client = new Client
            {
                Name = dto.Name,
                ContactDetails = dto.ContactDetails,
                Region = dto.Region
            };

            var createdClient = await _clientRepository.CreateAsync(client);

            return MapToDto(createdClient);
        }

        public async Task<bool> UpdateAsync(int id, UpdateClientDto dto)
        {
            var client = new Client
            {
                Id = id,
                Name = dto.Name,
                ContactDetails = dto.ContactDetails,
                Region = dto.Region
            };

            return await _clientRepository.UpdateAsync(client);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _clientRepository.DeleteAsync(id);
        }

        private static ClientDto MapToDto(Client client)
        {
            return new ClientDto
            {
                Id = client.Id,
                Name = client.Name,
                ContactDetails = client.ContactDetails,
                Region = client.Region
            };
        }
    }
}