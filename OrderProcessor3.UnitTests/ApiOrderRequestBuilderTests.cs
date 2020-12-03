using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OrderProcessor.UnitTests
{
    public class ApiOrderRequestBuilderTests
    {
        /// <summary>
        /// I'm adding this test to be complete but in reality I would not write a test for this. I
        /// would test it as a part of the ApiClient tests. You could say we're testing that the
        /// total is being calculated correctly. It's a judgment call.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task BuildRequest_AmountHasTaxAddedAndDataMatchesOrderObject()
        {
            // Given
            Mock<IOrderTaxCalculator> orderTaxCalculatorMock = new Mock<IOrderTaxCalculator>();
            orderTaxCalculatorMock.Setup(x => x.CalculateTotalWithTax(It.IsAny<double>())).Returns(108.25);
            ApiOrderRequestBuilder apiOrderRequestBuilder = new ApiOrderRequestBuilder(orderTaxCalculatorMock.Object);

            // When
            StringContent content = apiOrderRequestBuilder.BuildRequest(new Order(100, DateTime.Parse("2020-08-17"), 1));

            // Then
            string result = await content.ReadAsStringAsync();
            Order order = JsonConvert.DeserializeObject<Order>(result);
            order.Amount.Should().Be(108.25);
            order.DateSubmitted.Should().Be(DateTime.Parse("2020-08-17"));
            order.OrderId.Should().Be(1);
        }
    }
}