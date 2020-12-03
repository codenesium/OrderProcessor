using System.Threading.Tasks;

namespace OrderProcessor
{
    public class MockApiClient : IApiClient
    {
        public Task Process(Order order)
        {
            return Task.FromResult(true);
        }
    }
}