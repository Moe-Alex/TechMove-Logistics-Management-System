using TechMove.Models;
using TechMove.Models.Enums;

namespace TechMove.Api.Repositories
{
    public interface IContractRepository
    {
        Task<List<Contract>> GetAllAsync(ContractStatus? status, DateTime? startDateFrom, DateTime? endDateTo);
        Task<Contract?> GetByIdAsync(int id);
        Task<Contract> CreateAsync(Contract contract);
        Task<bool> UpdateStatusAsync(int id, ContractStatus status);
        Task<bool> ExistsAsync(int id);
    }
}