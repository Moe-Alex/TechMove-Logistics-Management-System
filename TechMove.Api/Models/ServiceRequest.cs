using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechMove.Models.Enums;

namespace TechMove.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [ForeignKey(nameof(ContractId))]
        public Contract? Contract { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal CostUSD { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal ExchangeRateUsed { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal LocalCostZAR { get; set; }

        [Required]
        public ServiceRequestStatus Status { get; set; }
    }
}