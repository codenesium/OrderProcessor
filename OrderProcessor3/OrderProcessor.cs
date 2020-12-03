using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderProcessor
{
    public interface IOrderProcessor
    {
        Task<ProcessResult> ProcessOrders(string inputFilename, string destinationDirectory);
    }

    public class OrderProcessor : IOrderProcessor
    {
        private readonly IOrderLoader orderLoader;
        private readonly IApiClient apiClient;
        private readonly ILogger<OrderProcessor> logger;

        public OrderProcessor(ILogger<OrderProcessor> logger, IOrderLoader orderLoader, IApiClient apiClient)
        {
            this.logger = logger;
            this.orderLoader = orderLoader;
            this.apiClient = apiClient;
        }

        public async Task<ProcessResult> ProcessOrders(string ordersDirectory, string destinationDirectory)
        {
            List<Order> orders = this.orderLoader.LoadOrders(ordersDirectory);

            foreach (Order order in orders)
            {
                await this.apiClient.Process(order);
                this.orderLoader.CompleteOrder(order.OrderId, ordersDirectory, destinationDirectory);
                logger.LogInformation($"Order Processed:{order.OrderId}");
            }

            return new ProcessResult();
        }
    }
}