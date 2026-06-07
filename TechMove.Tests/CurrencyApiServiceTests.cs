using System;
using TechMove.Services;
using Xunit;

namespace TechMove.Tests
{
    public class CurrencyApiServiceTests
    {
        [Fact]
        public void ConvertUsdToZar_ShouldReturnCorrectConvertedAmount()
        {
            // Arrange
            var service = new CurrencyApiService(null!, null!);
            decimal usdAmount = 10m;
            decimal exchangeRate = 18.50m;

            // Act
            var result = service.ConvertUsdToZar(usdAmount, exchangeRate);

            // Assert
            Assert.Equal(185.00m, result);
        }

        [Fact]
        public void ConvertUsdToZar_ShouldReturnZero_WhenUsdAmountIsZero()
        {
            // Arrange
            var service = new CurrencyApiService(null!, null!);
            decimal usdAmount = 0m;
            decimal exchangeRate = 18.50m;

            // Act
            var result = service.ConvertUsdToZar(usdAmount, exchangeRate);

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ConvertUsdToZar_ShouldRoundToTwoDecimalPlaces()
        {
            // Arrange
            var service = new CurrencyApiService(null!, null!);
            decimal usdAmount = 5.555m;
            decimal exchangeRate = 18.123456m;

            // Act
            var result = service.ConvertUsdToZar(usdAmount, exchangeRate);

            // Assert
            Assert.Equal(Math.Round(usdAmount * exchangeRate, 2), result);
        }
    }
}