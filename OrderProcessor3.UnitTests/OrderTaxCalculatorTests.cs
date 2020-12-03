using FluentAssertions;
using Xunit;

namespace OrderProcessor.UnitTests
{
    public class OrderTaxCalculatorTests
    {
        [Fact]
        public void CalculateTotalWithTaxTest()
        {
            // Given
            OrderTaxCalculator orderTaxCalculator = new OrderTaxCalculator(new Settings()
            {
                TaxRatePercent = 8.25,
            });

            // When
            double total = orderTaxCalculator.CalculateTotalWithTax(100);

            // Then
            total.Should().Be(108.25);
        }
    }
}