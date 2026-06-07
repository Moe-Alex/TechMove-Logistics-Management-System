using TechMove.Models.Enums;
using TechMove.Services;
using Xunit;

namespace TechMove.Tests
{
    public class ContractValidationServiceTests
    {
        [Fact]
        public void CanCreateServiceRequest_ShouldReturnTrue_ForDraftContract()
        {
            // Arrange
            var service = new ContractValidationService();

            // Act
            var result = service.CanCreateServiceRequest(ContractStatus.Draft);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanCreateServiceRequest_ShouldReturnTrue_ForActiveContract()
        {
            // Arrange
            var service = new ContractValidationService();

            // Act
            var result = service.CanCreateServiceRequest(ContractStatus.Active);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanCreateServiceRequest_ShouldReturnFalse_ForExpiredContract()
        {
            // Arrange
            var service = new ContractValidationService();

            // Act
            var result = service.CanCreateServiceRequest(ContractStatus.Expired);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanCreateServiceRequest_ShouldReturnFalse_ForOnHoldContract()
        {
            // Arrange
            var service = new ContractValidationService();

            // Act
            var result = service.CanCreateServiceRequest(ContractStatus.OnHold);

            // Assert
            Assert.False(result);
        }
    }
}