using TechMove.Models.Enums;

namespace TechMove.Models.ViewModels
{
    public class ContractFilterViewModel
    {
        public ContractStatus? Status { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
    }
}