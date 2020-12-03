using System.Net.Http;
using System.Threading.Tasks;

namespace OrderProcessor
{
    public interface IApiClient
    {
        Task<bool> Process(Order order);
    }

    public class ApiClient : IApiClient
    {
        private readonly IApiOrderRequestBuilder requestBuilder;

        public ApiClient(IApiOrderRequestBuilder requestBuilder)
        {
            this.requestBuilder = requestBuilder;
        }

        public async Task<bool> Process(Order order)
        {
            StringContent request = this.requestBuilder.BuildRequest(order);
            bool apiSuccess = await this.CallApi(request);
            return apiSuccess;
        }

        private async Task<bool> CallApi(StringContent request)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync("http://www.visa.com/api/payment/process", request);
            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}