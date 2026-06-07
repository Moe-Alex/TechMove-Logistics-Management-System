using System.Net;
using System.Net.Http.Json;
using TechMove.ApiDtos;

namespace TechMove.ApiClients
{
    public class ClientApiClient : IClientApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ClientDto>> GetClientsAsync()
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var result = await client.GetFromJsonAsync<List<ClientDto>>("api/clients");

            return result ?? new List<ClientDto>();
        }

        public async Task<ClientDto?> GetClientByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.GetAsync($"api/clients/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ClientDto>();
        }

        public async Task<ClientDto?> CreateClientAsync(CreateClientDto dto)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.PostAsJsonAsync("api/clients", dto);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ClientDto>();
        }

        public async Task<bool> UpdateClientAsync(int id, UpdateClientDto dto)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.PutAsJsonAsync($"api/clients/{id}", dto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.DeleteAsync($"api/clients/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}