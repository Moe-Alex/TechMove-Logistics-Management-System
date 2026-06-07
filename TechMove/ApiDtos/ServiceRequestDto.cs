using TechMove.Models.Enums;

namespace TechMove.ApiDtos
{
    public class ServiceRequestDto
    {
        public int Id { get; set; }

        public int ContractId { get; set; }

        public string ClientName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal CostUSD { get; set; }

        public decimal ExchangeRateUsed { get; set; }

        public decimal LocalCostZAR { get; set; }

        public ServiceRequestStatus Status { get; set; }
    }
}