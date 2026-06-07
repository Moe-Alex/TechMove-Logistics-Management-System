using TechMove.Api.DTOs;
using TechMove.Api.Repositories;
using TechMove.Models;

namespace TechMove.Api.Services
{
    public class ContractApiService : IContractApiService
    {
        private readonly IContractRepository _contractRepository;

        public ContractApiService(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<List<ContractDto>> GetContractsAsync(
            Models.Enums.ContractStatus? status,
            DateTime? startDateFrom,
            DateTime? endDateTo)
        {
            var contracts = await _contractRepository.GetAllAsync(status, startDateFrom, endDateTo);

            return contracts.Select(MapToDto).ToList();
        }

        public async Task<ContractDto?> GetContractByIdAsync(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
            {
                return null;
            }

            return MapToDto(contract);
        }

        public async Task<ContractDto> CreateContractAsync(CreateContractDto dto)
        {
            if (dto.EndDate < dto.StartDate)
            {
                throw new ArgumentException("End date cannot be before start date.");
            }

            var contract = new Contract
            {
                ClientId = dto.ClientId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                ServiceLevel = dto.ServiceLevel
            };

            var createdContract = await _contractRepository.CreateAsync(contract);

            return MapToDto(createdContract);
        }

        public async Task<bool> UpdateContractStatusAsync(int id, UpdateContractStatusDto dto)
        {
            return await _contractRepository.UpdateStatusAsync(id, dto.Status);
        }

        private static ContractDto MapToDto(Contract contract)
        {
            return new ContractDto
            {
                Id = contract.Id,
                ClientId = contract.ClientId,
                ClientName = contract.Client?.Name ?? string.Empty,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status,
                ServiceLevel = contract.ServiceLevel,
                SignedAgreementFilePath = contract.SignedAgreementFilePath,
                SignedAgreementOriginalFileName = contract.SignedAgreementOriginalFileName
            };
        }
    }
}