using System.Collections.Generic;
using System.IO;

namespace OrderProcessor
{
    public interface IOrderLoader
    {
        void CompleteOrder(int orderId, string orderDirectory, string destinationDirectory);

        List<Order> LoadOrders(string orderDirectory);
    }

    public class OrderLoader : IOrderLoader
    {
        private readonly IOrderParser orderParser;
        private readonly IFileSystem fileSystem;

        public OrderLoader(IOrderParser orderParser, IFileSystem fileSystem)
        {
            this.orderParser = orderParser;
            this.fileSystem = fileSystem;
        }

        public List<Order> LoadOrders(string orderDirectory)
        {
            List<string> records = this.LoadRecords(orderDirectory);
            List<Order> orders = this.orderParser.Parse(records);
            return orders;
        }

        public void CompleteOrder(int orderId, string orderDirectory, string destinationDirectory)
        {
            string sourceFilename = Path.Combine(orderDirectory, $"{orderId}.csv");
            string destinationFilename = Path.Combine(destinationDirectory, $"{orderId}.csv");
            this.fileSystem.MoveFile(sourceFilename, destinationFilename);
        }

        private List<string> LoadRecords(string orderDirectory)
        {
            string[] files = this.fileSystem.GetFiles(orderDirectory);
            List<string> records = new List<string>();

            foreach (string file in files)
            {
                records.Add(this.fileSystem.ReadAllText(file));
            }

            return records;
        }
    }
}