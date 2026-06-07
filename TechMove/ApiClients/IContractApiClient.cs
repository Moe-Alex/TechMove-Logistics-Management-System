using TechMove.ApiDtos;
using TechMove.Models.Enums;

namespace TechMove.ApiClients
{
    public interface IContractApiClient
    {
        Task<List<ContractDto>> GetContractsAsync(ContractStatus? status, DateTime? startDateFrom, DateTime? endDateTo);
        Task<ContractDto?> GetContractByIdAsync(int id);
        Task<ContractDto?> CreateContractAsync(CreateContractDto dto);
        Task<bool> UpdateContractAsync(int id, UpdateContractDto dto);
        Task<bool> UpdateContractStatusAsync(int id, UpdateContractStatusDto dto);
        Task<bool> UploadAgreementAsync(int id, IFormFile file);
        Task<byte[]?> DownloadAgreementAsync(int id);
        Task<bool> DeleteContractAsync(int id);
    }
}