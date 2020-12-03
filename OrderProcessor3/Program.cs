using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace OrderProcessor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // create a configuration object using an appsettings.json file
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ServiceCollection services = ConfigureServices(configuration);

            // build a provider that we can resolve services from
            ServiceProvider provider = services.BuildServiceProvider();

            // resolve an IOrderProcessor
            IOrderProcessor orderProcessor = provider.GetRequiredService<IOrderProcessor>();

            // process our orders
            Settings apiSettings = provider.GetRequiredService<Settings>();
            await orderProcessor.ProcessOrders(apiSettings.OrderDirectory, apiSettings.DestinationDirectory);
        }

        private static ServiceCollection ConfigureServices(IConfigurationRoot configuration)
        {
            // load any settings we need into a Settings object
            Settings apiSettings = configuration.GetSection("Settings").Get<Settings>();

            // create and register our services we want the IOC container to be able to inject
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure => configure.AddConsole());
            serviceCollection.AddTransient<IFileSystem, FileSystem>();
            serviceCollection.AddTransient<IOrderParser, OrderParser>();
            serviceCollection.AddTransient<IOrderLoader, OrderLoader>();
            serviceCollection.AddTransient<IOrderProcessor, OrderProcessor>();

            // add a single instance of our settings to be used across the application
            serviceCollection.AddSingleton<Settings>(apiSettings);
            // use a mock api client when debugging so we can test without calling the real API
#if DEBUG
            serviceCollection.AddTransient<IApiClient, MockApiClient>();
#else
            serviceCollection.AddTransient<IApiClient, ApiClient>();
#endif
            serviceCollection.AddTransient<IApiOrderRequestBuilder, ApiOrderRequestBuilder>();
            serviceCollection.AddTransient<IOrderTaxCalculator, OrderTaxCalculator>();

            serviceCollection.AddHttpClient();

            return serviceCollection;
        }
    }
}