using System.Text.Json;

namespace TechMove.Services
{
    public class CurrencyApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CurrencyApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<decimal?> GetUsdToZarRateAsync()
        {
            try
            {
                var baseUrl = _configuration["ExchangeRateApi:BaseUrl"];
                var apiKey = _configuration["ExchangeRateApi:ApiKey"];

                if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
                {
                    return null;
                }

                var requestUrl = $"{baseUrl.TrimEnd('/')}/{apiKey}/latest/USD";

                var response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(json);

                if (!document.RootElement.TryGetProperty("conversion_rates", out var conversionRates))
                {
                    return null;
                }

                if (!conversionRates.TryGetProperty("ZAR", out var zarRateElement))
                {
                    return null;
                }

                return zarRateElement.GetDecimal();
            }
            catch
            {
                return null;
            }
        }

        public decimal ConvertUsdToZar(decimal amountUsd, decimal exchangeRate)
        {
            return Math.Round(amountUsd * exchangeRate, 2);
        }
    }
}