using Newtonsoft.Json;
using System.Net.Http;

namespace OrderProcessor
{
    public interface IApiOrderRequestBuilder
    {
        StringContent BuildRequest(Order order);
    }

    public class ApiOrderRequestBuilder : IApiOrderRequestBuilder
    {
        private readonly IOrderTaxCalculator orderTaxCalculator;

        public ApiOrderRequestBuilder(IOrderTaxCalculator orderTaxCalculator)
        {
            this.orderTaxCalculator = orderTaxCalculator;
        }

        public StringContent BuildRequest(Order order)
        {
            StringContent request = new StringContent(JsonConvert.SerializeObject(new
            {
                Amount = orderTaxCalculator.CalculateTotalWithTax(order.Amount),
                order.OrderId,
                order.DateSubmitted
            }));
            return request;
        }
    }
}