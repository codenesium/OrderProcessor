using System.Threading.Tasks;

namespace OrderProcessor
{
    public class MockApiClient : IApiClient
    {
        public Task<bool> Process(Order order)
        {
            return Task.FromResult(true);
        }
    }
}