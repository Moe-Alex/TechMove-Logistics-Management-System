using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.Models.Enums;

namespace TechMove.Models.ViewModels
{
    public class ServiceRequestCreateViewModel
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        public List<SelectListItem> Contracts { get; set; } = new();

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal CostUSD { get; set; }

        public decimal ExchangeRateUsed { get; set; }

        public decimal LocalCostZAR { get; set; }

        [Required]
        public ServiceRequestStatus Status { get; set; }
    }
}