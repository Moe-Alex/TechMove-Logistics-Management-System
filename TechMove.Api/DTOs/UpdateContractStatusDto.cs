using System.ComponentModel.DataAnnotations;
using TechMove.Models.Enums;

namespace TechMove.Api.DTOs
{
    public class UpdateContractStatusDto
    {
        [Required]
        public ContractStatus Status { get; set; }
    }
}