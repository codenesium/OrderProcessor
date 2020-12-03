using System;
using System.Collections.Generic;

namespace OrderProcessor
{
    public interface IOrderParser
    {
        List<Order> Parse(List<string> records);
    }

    public class OrderParser : IOrderParser
    {
        public List<Order> Parse(List<string> records)
        {
            List<Order> response = new List<Order>();
            foreach (string record in records)
            {
                response.Add(this.ParseRecord(record));
            }
            return response;
        }

        private Order ParseRecord(string record)
        {
            string[] parsedRecord = record.Split(',');

            return new Order(int.Parse(parsedRecord[2]),
                 DateTime.Parse(parsedRecord[1]),
                  int.Parse(parsedRecord[0]));
        }
    }
}