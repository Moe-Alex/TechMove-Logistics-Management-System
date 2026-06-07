using TechMove.Models.Enums;

namespace TechMove.Api.DTOs
{
    public class ContractDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; }
        public string ServiceLevel { get; set; } = string.Empty;
        public string? SignedAgreementFilePath { get; set; }
        public string? SignedAgreementOriginalFileName { get; set; }
    }
}