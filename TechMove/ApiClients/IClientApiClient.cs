using TechMove.ApiDtos;

namespace TechMove.ApiClients
{
    public interface IClientApiClient
    {
        Task<List<ClientDto>> GetClientsAsync();
        Task<ClientDto?> GetClientByIdAsync(int id);
        Task<ClientDto?> CreateClientAsync(CreateClientDto dto);
        Task<bool> UpdateClientAsync(int id, UpdateClientDto dto);
        Task<bool> DeleteClientAsync(int id);
    }
}