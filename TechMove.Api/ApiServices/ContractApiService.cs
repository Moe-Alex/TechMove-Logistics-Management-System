using TechMove.Api.DTOs;
using TechMove.Api.Repositories;
using TechMove.Models;
using TechMove.Services;

namespace TechMove.Api.Services
{
    public class ContractApiService : IContractApiService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IClientRepository _clientRepository;
        private readonly FileService _fileService;

        public ContractApiService(
            IContractRepository contractRepository,
            IClientRepository clientRepository,
            FileService fileService)
        {
            _contractRepository = contractRepository;
            _clientRepository = clientRepository;
            _fileService = fileService;
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
            var clientExists = await _clientRepository.ExistsAsync(dto.ClientId);

            if (!clientExists)
            {
                throw new ArgumentException("Selected client was not found.");
            }

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
            var createdWithClient = await _contractRepository.GetByIdAsync(createdContract.Id);

            return MapToDto(createdWithClient!);
        }

        public async Task<bool> UpdateContractAsync(int id, UpdateContractDto dto)
        {
            var clientExists = await _clientRepository.ExistsAsync(dto.ClientId);

            if (!clientExists)
            {
                throw new ArgumentException("Selected client was not found.");
            }

            if (dto.EndDate < dto.StartDate)
            {
                throw new ArgumentException("End date cannot be before start date.");
            }

            var contract = new Contract
            {
                Id = id,
                ClientId = dto.ClientId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                ServiceLevel = dto.ServiceLevel
            };

            return await _contractRepository.UpdateAsync(contract);
        }

        public async Task<bool> UpdateContractStatusAsync(int id, UpdateContractStatusDto dto)
        {
            return await _contractRepository.UpdateStatusAsync(id, dto.Status);
        }

        public async Task<bool> UploadAgreementAsync(int id, IFormFile file)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
            {
                return false;
            }

            var saveResult = await _fileService.SavePdfAsync(file);

            if (!saveResult.Success)
            {
                throw new ArgumentException(saveResult.ErrorMessage);
            }

            return await _contractRepository.UpdateAgreementAsync(
                id,
                saveResult.FilePath!,
                saveResult.OriginalFileName!);
        }

        public async Task<(byte[] FileBytes, string FileName)?> DownloadAgreementAsync(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(contract.SignedAgreementFilePath))
            {
                return null;
            }

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                contract.SignedAgreementFilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!File.Exists(filePath))
            {
                return null;
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);

            var fileName = string.IsNullOrWhiteSpace(contract.SignedAgreementOriginalFileName)
                ? "signed-agreement.pdf"
                : contract.SignedAgreementOriginalFileName;

            return (fileBytes, fileName);
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            return await _contractRepository.DeleteAsync(id);
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