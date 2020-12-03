using System;

namespace OrderProcessor
{
    public class Order
    {
        public double Amount { get; private set; }

        public DateTime DateSubmitted { get; private set; }

        public int OrderId { get; private set; }

        public Order(double amount, DateTime dateSubmitted, int orderId)
        {
            Amount = amount;
            DateSubmitted = dateSubmitted;
            OrderId = orderId;
        }

        public string ToDebugString()
        {
            return $@"OrderId={this.OrderId},Amount={this.Amount},DateSubmitted={this.DateSubmitted}";
        }
    }
}