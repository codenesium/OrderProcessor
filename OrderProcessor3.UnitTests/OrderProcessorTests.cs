using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OrderProcessor.UnitTests
{
    public class OrderProcessorTests
    {
        [Fact()]
        public async Task ProcessOrdersTest()
        {
            // Given

            Mock<ILogger<OrderProcessor>> loggerStub = new Mock<ILogger<OrderProcessor>>();
            Mock<IOrderLoader> orderLoaderMock = new Mock<IOrderLoader>();
            orderLoaderMock.Setup(x => x.LoadOrders(It.IsAny<string>())).Returns(new
                 List<Order>()
                {
                    new Order(100,DateTime.Parse("2001-01-01"), 1),
                });
            Mock<IApiClient> apiClientMock = new Mock<IApiClient>();

            OrderProcessor orderProcessor = new OrderProcessor(
                loggerStub.Object,
                orderLoaderMock.Object,
                apiClientMock.Object);

            // When

            ProcessResult result = await orderProcessor.ProcessOrders(@"c:\unprocessed\orders.txt", @"c:\processed\orders.txt");

            // Then
            result.Success.Should().BeTrue();

            // It's usually a bad idea to verify the internals of a test. It makes your test
            // brittle. You should be testing interface not implementation. I'm leaving this here as
            // an example but a better practice would be to test this in an integration test or not
            // to test this detail.
            apiClientMock.Verify(x => x.Process(It.IsAny<Order>()));
        }
    }
}