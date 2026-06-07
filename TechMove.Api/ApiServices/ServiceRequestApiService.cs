using TechMove.Api.DTOs;
using TechMove.Api.Repositories;
using TechMove.Models;
using TechMove.Services;

namespace TechMove.Api.Services
{
    public class ServiceRequestApiService : IServiceRequestApiService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ContractValidationService _contractValidationService;
        private readonly CurrencyApiService _currencyApiService;

        public ServiceRequestApiService(
            IServiceRequestRepository serviceRequestRepository,
            IContractRepository contractRepository,
            ContractValidationService contractValidationService,
            CurrencyApiService currencyApiService)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _contractRepository = contractRepository;
            _contractValidationService = contractValidationService;
            _currencyApiService = currencyApiService;
        }

        public async Task<List<ServiceRequestDto>> GetAllAsync()
        {
            var serviceRequests = await _serviceRequestRepository.GetAllAsync();

            return serviceRequests.Select(MapToDto).ToList();
        }

        public async Task<ServiceRequestDto?> GetByIdAsync(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (serviceRequest == null)
            {
                return null;
            }

            return MapToDto(serviceRequest);
        }

        public async Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto)
        {
            var contract = await _contractRepository.GetByIdAsync(dto.ContractId);

            if (contract == null)
            {
                throw new ArgumentException("Selected contract was not found.");
            }

            if (!_contractValidationService.CanCreateServiceRequest(contract.Status))
            {
                throw new InvalidOperationException("A service request cannot be created for an Expired or On Hold contract.");
            }

            var rate = await _currencyApiService.GetUsdToZarRateAsync();

            if (rate == null)
            {
                throw new InvalidOperationException("Could not retrieve the exchange rate at this time.");
            }

            var serviceRequest = new ServiceRequest
            {
                ContractId = dto.ContractId,
                Description = dto.Description,
                CostUSD = dto.CostUSD,
                ExchangeRateUsed = rate.Value,
                LocalCostZAR = _currencyApiService.ConvertUsdToZar(dto.CostUSD, rate.Value),
                Status = dto.Status
            };

            var createdServiceRequest = await _serviceRequestRepository.CreateAsync(serviceRequest);

            var createdWithDetails = await _serviceRequestRepository.GetByIdAsync(createdServiceRequest.Id);

            return MapToDto(createdWithDetails!);
        }

        public async Task<bool> UpdateAsync(int id, UpdateServiceRequestDto dto)
        {
            var existingServiceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (existingServiceRequest == null)
            {
                return false;
            }

            var contract = await _contractRepository.GetByIdAsync(dto.ContractId);

            if (contract == null)
            {
                throw new ArgumentException("Selected contract was not found.");
            }

            if (!_contractValidationService.CanCreateServiceRequest(contract.Status))
            {
                throw new InvalidOperationException("A service request cannot be linked to an Expired or On Hold contract.");
            }

            var rate = await _currencyApiService.GetUsdToZarRateAsync();

            if (rate == null)
            {
                throw new InvalidOperationException("Could not retrieve the exchange rate at this time.");
            }

            var updatedServiceRequest = new ServiceRequest
            {
                Id = id,
                ContractId = dto.ContractId,
                Description = dto.Description,
                CostUSD = dto.CostUSD,
                ExchangeRateUsed = rate.Value,
                LocalCostZAR = _currencyApiService.ConvertUsdToZar(dto.CostUSD, rate.Value),
                Status = dto.Status
            };

            return await _serviceRequestRepository.UpdateAsync(updatedServiceRequest);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _serviceRequestRepository.DeleteAsync(id);
        }

        private static ServiceRequestDto MapToDto(ServiceRequest serviceRequest)
        {
            return new ServiceRequestDto
            {
                Id = serviceRequest.Id,
                ContractId = serviceRequest.ContractId,
                ClientName = serviceRequest.Contract?.Client?.Name ?? string.Empty,
                Description = serviceRequest.Description,
                CostUSD = serviceRequest.CostUSD,
                ExchangeRateUsed = serviceRequest.ExchangeRateUsed,
                LocalCostZAR = serviceRequest.LocalCostZAR,
                Status = serviceRequest.Status
            };
        }
    }
}