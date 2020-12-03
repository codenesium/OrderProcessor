using System;

namespace OrderProcessor
{
    public interface IOrderTaxCalculator
    {
        double CalculateTotalWithTax(double total);
    }

    public class OrderTaxCalculator : IOrderTaxCalculator
    {
        private readonly Settings settings;

        public OrderTaxCalculator(Settings settings)
        {
            this.settings = settings;
        }

        public double CalculateTotalWithTax(double total)
        {
            double totalAmount = Math.Round(total * (1 + (this.settings.TaxRatePercent / 100)), 2);
            return totalAmount;
        }
    }
}