using TechMove.ApiDtos;

namespace TechMove.ApiClients
{
    public interface IServiceRequestApiClient
    {
        Task<List<ServiceRequestDto>> GetServiceRequestsAsync();
        Task<ServiceRequestDto?> GetServiceRequestByIdAsync(int id);
        Task<ServiceRequestDto?> CreateServiceRequestAsync(CreateServiceRequestDto dto);
        Task<bool> UpdateServiceRequestAsync(int id, UpdateServiceRequestDto dto);
        Task<bool> DeleteServiceRequestAsync(int id);
    }
}