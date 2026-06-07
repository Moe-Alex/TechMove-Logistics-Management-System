using TechMove.Api.DTOs;

namespace TechMove.Api.Services
{
    public interface IServiceRequestApiService
    {
        Task<List<ServiceRequestDto>> GetAllAsync();
        Task<ServiceRequestDto?> GetByIdAsync(int id);
        Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto);
        Task<bool> UpdateAsync(int id, UpdateServiceRequestDto dto);
        Task<bool> DeleteAsync(int id);
    }
}