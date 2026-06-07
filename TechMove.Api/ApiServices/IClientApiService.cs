using TechMove.Api.DTOs;

namespace TechMove.Api.Services
{
    public interface IClientApiService
    {
        Task<List<ClientDto>> GetAllAsync();
        Task<ClientDto?> GetByIdAsync(int id);
        Task<ClientDto> CreateAsync(CreateClientDto dto);
        Task<bool> UpdateAsync(int id, UpdateClientDto dto);
        Task<bool> DeleteAsync(int id);
    }
}