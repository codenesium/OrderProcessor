using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OrderProcessor.IntegrationTests
{
    public class ApiClientTests
    {
        [Fact]
        public async Task Process_HttpClientiIsCalledWithTheCorrectParameters()
        {
            // Given
            Settings settings = new Settings()
            {
                ApiEndpoint = "http://localhost/api/process",
            };
            Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
            Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK
               })
               .Verifiable();

            HttpClient httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(_ => _.CreateClient(string.Empty))
            .Returns(httpClient).Verifiable();

            ApiClient apiClient = new ApiClient(
                new ApiOrderRequestBuilder(new OrderTaxCalculator(settings)),
                settings,
               httpClientFactoryMock.Object);

            // When
            Order order = new Order(100, DateTime.Parse("2001-01-01"), 1);
            await apiClient.Process(order);

            // Then
            Func<HttpRequestMessage, bool> verify = (req) =>
             {
                 string content = req.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                 Order apiOrder = JsonConvert.DeserializeObject<Order>(content);
                 apiOrder.Should().BeEquivalentTo(order);
                 req.Method.Should().Be(HttpMethod.Post);
                 req.RequestUri.ToString().Should().Be(settings.ApiEndpoint);
                 return true;
             };

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(x => verify(x)),
               ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}