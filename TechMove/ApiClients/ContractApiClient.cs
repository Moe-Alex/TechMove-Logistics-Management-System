using System.Net;
using System.Net.Http.Json;
using TechMove.ApiDtos;
using TechMove.Models.Enums;

namespace TechMove.ApiClients
{
    public class ContractApiClient : IContractApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ContractApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ContractDto>> GetContractsAsync(
            ContractStatus? status,
            DateTime? startDateFrom,
            DateTime? endDateTo)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var query = new List<string>();

            if (status.HasValue)
            {
                query.Add($"status={(int)status.Value}");
            }

            if (startDateFrom.HasValue)
            {
                query.Add($"startDateFrom={startDateFrom.Value:yyyy-MM-dd}");
            }

            if (endDateTo.HasValue)
            {
                query.Add($"endDateTo={endDateTo.Value:yyyy-MM-dd}");
            }

            var url = "api/contracts";

            if (query.Any())
            {
                url += "?" + string.Join("&", query);
            }

            var result = await client.GetFromJsonAsync<List<ContractDto>>(url);

            return result ?? new List<ContractDto>();
        }

        public async Task<ContractDto?> GetContractByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.GetAsync($"api/contracts/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ContractDto>();
        }

        public async Task<ContractDto?> CreateContractAsync(CreateContractDto dto)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.PostAsJsonAsync("api/contracts", dto);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ContractDto>();
        }

        public async Task<bool> UpdateContractAsync(int id, UpdateContractDto dto)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.PutAsJsonAsync($"api/contracts/{id}", dto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateContractStatusAsync(int id, UpdateContractStatusDto dto)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.PatchAsJsonAsync($"api/contracts/{id}/status", dto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UploadAgreementAsync(int id, IFormFile file)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            using var fileContent = new StreamContent(stream);

            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            var response = await client.PostAsync($"api/contracts/{id}/agreement", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<byte[]?> DownloadAgreementAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.GetAsync($"api/contracts/{id}/agreement");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.DeleteAsync($"api/contracts/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}