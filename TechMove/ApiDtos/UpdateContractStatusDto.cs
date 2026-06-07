using System.ComponentModel.DataAnnotations;
using TechMove.Models.Enums;

namespace TechMove.ApiDtos
{
    public class UpdateContractStatusDto
    {
        [Required]
        public ContractStatus Status { get; set; }
    }
}