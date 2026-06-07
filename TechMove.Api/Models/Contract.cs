using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechMove.Models.Enums;

namespace TechMove.Models
{
    public class Contract
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        [Required]
        [StringLength(100)]
        public string ServiceLevel { get; set; } = string.Empty;

        [StringLength(255)]
        public string? SignedAgreementFilePath { get; set; }

        [StringLength(255)]
        public string? SignedAgreementOriginalFileName { get; set; }

        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}