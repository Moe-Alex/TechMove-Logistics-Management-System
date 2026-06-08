using System.Net.Http.Json;
using TechMove.ApiDtos;

namespace TechMove.ApiClients
{
    public class FirebaseAuthClient : IFirebaseAuthClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public FirebaseAuthClient(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<FirebaseLoginResponse?> LoginAsync(string email, string password)
        {
            var apiKey = _configuration["Firebase:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Firebase API key is missing from appsettings.json.");
            }

            var client = _httpClientFactory.CreateClient();

            var request = new FirebaseLoginRequest
            {
                Email = email,
                Password = password,
                ReturnSecureToken = true
            };

            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

            var response = await client.PostAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<FirebaseLoginResponse>();
        }
    }
}