using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;

namespace TechMove.Api.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceRequest>> GetAllAsync()
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .OrderByDescending(sr => sr.Id)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task<ServiceRequest> CreateAsync(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return serviceRequest;
        }

        public async Task<bool> UpdateAsync(ServiceRequest serviceRequest)
        {
            var existingServiceRequest = await _context.ServiceRequests
                .FirstOrDefaultAsync(sr => sr.Id == serviceRequest.Id);

            if (existingServiceRequest == null)
            {
                return false;
            }

            existingServiceRequest.ContractId = serviceRequest.ContractId;
            existingServiceRequest.Description = serviceRequest.Description;
            existingServiceRequest.CostUSD = serviceRequest.CostUSD;
            existingServiceRequest.ExchangeRateUsed = serviceRequest.ExchangeRateUsed;
            existingServiceRequest.LocalCostZAR = serviceRequest.LocalCostZAR;
            existingServiceRequest.Status = serviceRequest.Status;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
            {
                return false;
            }

            _context.ServiceRequests.Remove(serviceRequest);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ServiceRequests.AnyAsync(sr => sr.Id == id);
        }
    }
}