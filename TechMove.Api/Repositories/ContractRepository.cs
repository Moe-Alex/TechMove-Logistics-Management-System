using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;
using TechMove.Models.Enums;

namespace TechMove.Api.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _context;

        public ContractRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contract>> GetAllAsync(
            ContractStatus? status,
            DateTime? startDateFrom,
            DateTime? endDateTo)
        {
            var query = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            if (startDateFrom.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDateFrom.Value);
            }

            if (endDateTo.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDateTo.Value);
            }

            return await query
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Contract> CreateAsync(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return contract;
        }

        public async Task<bool> UpdateAsync(Contract contract)
        {
            var existingContract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == contract.Id);

            if (existingContract == null)
            {
                return false;
            }

            existingContract.ClientId = contract.ClientId;
            existingContract.StartDate = contract.StartDate;
            existingContract.EndDate = contract.EndDate;
            existingContract.Status = contract.Status;
            existingContract.ServiceLevel = contract.ServiceLevel;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateStatusAsync(int id, ContractStatus status)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return false;
            }

            contract.Status = status;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAgreementAsync(int id, string filePath, string originalFileName)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return false;
            }

            contract.SignedAgreementFilePath = filePath;
            contract.SignedAgreementOriginalFileName = originalFileName;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
            {
                return false;
            }

            if (contract.ServiceRequests.Any())
            {
                throw new InvalidOperationException("This contract cannot be deleted because it has linked service requests.");
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Contracts.AnyAsync(c => c.Id == id);
        }
    }
}