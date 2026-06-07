using TechMove.Api.DTOs;
using TechMove.Models.Enums;

namespace TechMove.Api.Services
{
    public interface IContractApiService
    {
        Task<List<ContractDto>> GetContractsAsync(ContractStatus? status, DateTime? startDateFrom, DateTime? endDateTo);
        Task<ContractDto?> GetContractByIdAsync(int id);
        Task<ContractDto> CreateContractAsync(CreateContractDto dto);
        Task<bool> UpdateContractStatusAsync(int id, UpdateContractStatusDto dto);
    }
}