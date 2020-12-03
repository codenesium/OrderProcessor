using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrderProcessor
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var directories = Directory.GetFiles(@"C:\temp\orders\unprocessed");

            foreach (string file in directories)
            {
                string contents = File.ReadAllText(file);
                string[] record = contents.Split(',');

                double totalAmount = Math.Round(double.Parse(record[2]) * 1.08, 2);

                StringContent order = new StringContent(JsonConvert.SerializeObject(new
                {
                    Amount = totalAmount,
                    OrderId = record[0],
                    DateSubmitted = DateTime.Parse(record[1])
                }));

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.PostAsync("http://www.visa.com/api/payment/process", order);

                Console.WriteLine($"Order Processed:{record[0]}");

                File.Move(file, Path.Combine(@"C:\temp\orders\processed", Path.GetFileName(file)));
            }


        }
    }
}