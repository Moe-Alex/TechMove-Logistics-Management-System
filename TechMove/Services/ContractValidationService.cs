using TechMove.Models.Enums;

namespace TechMove.Services
{
    public class ContractValidationService
    {
        public bool CanCreateServiceRequest(ContractStatus contractStatus)
        {
            return contractStatus != ContractStatus.Expired &&
                   contractStatus != ContractStatus.OnHold;
        }
    }
}