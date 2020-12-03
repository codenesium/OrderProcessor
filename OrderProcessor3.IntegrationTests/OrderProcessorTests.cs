using FluentAssertions;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OrderProcessor.IntegrationTests
{
    public class OrderProcessorTests
    {
        [Fact]
        public async Task ProcessOrders_EndToEndTestWithMockedApiClient()
        {
            // Given
            string testDirectory = @"C:\temp\orders";
            string filename = "1.csv";
            string orderFilename = Path.Combine(testDirectory, "unprocessed", filename);
            string destinationDirectory = Path.Combine(testDirectory, "processed");
            string destinationFilename = Path.Combine(destinationDirectory, $"{filename}");
            Directory.CreateDirectory(testDirectory);
            Directory.CreateDirectory(destinationDirectory);
            File.Delete(orderFilename);
            File.Delete(destinationFilename);
            File.WriteAllLines(orderFilename,
                new string[]
                {
                    "1,2020-08-17,100"
                });

            // When
            await Program.Main(new string[] { });

            // Then
            File.Exists(orderFilename).Should().BeFalse();
            File.Exists(destinationFilename).Should().BeTrue();
        }
    }
}