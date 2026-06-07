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

                var requestUrl = $"https://v6.exchangerate-api.com/v6/b24f1c18771f1fe939e085a9/latest/USD";

                var response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(json);

                if (document.RootElement.TryGetProperty("conversion_rates", out var conversionRates) &&
                    conversionRates.TryGetProperty("ZAR", out var zarRate))
                {
                    return zarRate.GetDecimal();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public decimal ConvertUsdToZar(decimal usdAmount, decimal exchangeRate)
        {
            return Math.Round(usdAmount * exchangeRate, 2);
        }
    }
}