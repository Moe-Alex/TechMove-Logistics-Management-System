using System.Net;
using System.Net.Http.Json;
using TechMove.ApiDtos;

namespace TechMove.ApiClients
{
    public class ServiceRequestApiClient : IServiceRequestApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ServiceRequestApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ServiceRequestDto>> GetServiceRequestsAsync()
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var result = await client.GetFromJsonAsync<List<ServiceRequestDto>>("api/service-requests");

            return result ?? new List<ServiceRequestDto>();
        }

        public async Task<ServiceRequestDto?> GetServiceRequestByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.GetAsync($"api/service-requests/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ServiceRequestDto>();
        }

        public async Task<ServiceRequestDto?> CreateServiceRequestAsync(CreateServiceRequestDto dto)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.PostAsJsonAsync("api/service-requests", dto);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ServiceRequestDto>();
        }

        public async Task<bool> UpdateServiceRequestAsync(int id, UpdateServiceRequestDto dto)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.PutAsJsonAsync($"api/service-requests/{id}", dto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteServiceRequestAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.DeleteAsync($"api/service-requests/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}