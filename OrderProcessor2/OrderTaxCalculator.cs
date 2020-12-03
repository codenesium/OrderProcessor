using System;

namespace OrderProcessor
{
    public interface IOrderTaxCalculator
    {
        double CalculateTotalWithTax(double total);
    }

    public class OrderTaxCalculator : IOrderTaxCalculator
    {
        public double CalculateTotalWithTax(double total)
        {
            double totalAmount = Math.Round(total * 1.08, 2);
            return totalAmount;
        }
    }
}