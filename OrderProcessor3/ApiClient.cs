using Polly;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrderProcessor
{
    public interface IApiClient
    {
        Task Process(Order order);
    }

    public class ApiClient : IApiClient
    {
        private readonly IApiOrderRequestBuilder requestBuilder;
        private readonly Settings settings;
        private readonly IHttpClientFactory httpClientFactory;

        public ApiClient(IApiOrderRequestBuilder requestBuilder, Settings settings, IHttpClientFactory httpClientFactory)
        {
            this.requestBuilder = requestBuilder;
            this.settings = settings;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task Process(Order order)
        {
            StringContent request = this.requestBuilder.BuildRequest(order);
            await this.CallApi(request);
        }

        private async Task CallApi(StringContent request)
        {
            AsyncPolicy policy = Policy
              .Handle<Exception>()
              .WaitAndRetryAsync(new[]
              {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3)
              });

            Func<Task> callApi = async () =>
            {
                HttpClient client = this.httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.PostAsync(this.settings.ApiEndpoint, request);
                response.EnsureSuccessStatusCode();
            };

            await policy.ExecuteAsync(() => callApi());
        }
    }
}