The first thing we want to do is create a strongly typed object to represent our orders instead of
using a string array.

```
public class Order
{
    public double Amount { get; private set; }

    public DateTime DateSubmitted { get; private set; }

    public int OrderId { get; private set; }

    public Record(double amount, DateTime dateSubmitted, int orderId)
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
```

and while we're at it an order parser class to translate our CSV records. With a separte class
for parsing records into record objects we can segregate this responsibility. 

```
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
```

You may be asking what is IOrderParser?

IOrderParser is the interface for our OrderParser class. It describes how our class functions. 
In the SOLID principles there is the concept known as dependency inversion. What this means is 
things that a class depends on are are injected in to the class instead of creating the instance from
inside the class. This is commonly referred to as dependency injection. 

IOrderParser.cs looks like this.

```
public interface IOrderParser
{
    List<Order> Parse(List<string> records);
}
```

Let's add one more class to showcase dependency inversion.
You may be wondering what IFileSystem is. This is a class that decorates the .NET static
methods like File.ReadAllLines. Later when we discuss testing we will dive into why
you want to wrap these methods in a class.

```
public class OrderLoader
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
```
So what is happening here?

In the constructor we pass two parameters. Where do these come from...It's magic.
```
public OrderLoader(IOrderParser orderParser, IFileSystem fileSystem)
{
    this.orderParser = orderParser;
    this.fileSystem = fileSystem;
}
```
Read up here on the details https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0

What happens is there is an object that lives in your application that knows how to wire up
any object it knows about. We register IFileSystem, IOrderParser and OrderLoader and this inversion
of control container knows how to create an OrderLoader object and it knows to create a IFileSystem
and a IOrderParser and pass them to our OrderLoader. Like I said it's magic. You don't need to know
a lot of details about this other than how to set up the IOC system and what scopes mean.

Scope determines if a new instance is created every time you need a type.

Singleton -> there is one instance. Every oject that needs an instance gets the same one.
Scoped -> The same instance is used for a scope. This is used in ASP.NET requests so that the entire
request uses the same instance.
Transient -> every time a type is request a new one is created. This is the most common scope you will use.

To create a IOC container in .NET Core

Install the package Microsoft.Extensions.DependencyInjection.

Then in your startup method
```
private static async Task Main(string[] args)
{
    // create a configuration object using an appsettings.json file
    IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    // create and register our services we want the IOC container to be able to inject
    ServiceCollection serviceCollection = new ServiceCollection();
    serviceCollection.AddLogging(configure => configure.AddConsole());
    serviceCollection.AddTransient<IFileSystem, FileSystem>();
    serviceCollection.AddTransient<IOrderParser, OrderParser>();
    serviceCollection.AddTransient<IOrderLoader, OrderLoader>();
    serviceCollection.AddTransient<IOrderProcessor, OrderProcessor>();

    // use a mock api client when debugging so we can test without calling the real API
#if DEBUG
    serviceCollection.AddTransient<IApiClient, MockApiClient>();
#else
    serviceCollection.AddTransient<IApiClient, ApiClient>();
#endif
    serviceCollection.AddTransient<IApiOrderRequestBuilder, ApiOrderRequestBuilder>();
    serviceCollection.AddTransient<IOrderTaxCalculator, OrderTaxCalculator>();
            
    // build a provider that we can resolve services from
    ServiceProvider provider = serviceCollection.BuildServiceProvider();

    // resolve an IOrderProcessor
    IOrderProcessor orderProcessor = provider.GetRequiredService<IOrderProcessor>();

    // process our orders
    await orderProcessor.ProcessOrders(@"C:\temp\orders\unprocessed\orders.csv", @"C:\temp\orders\processed\orders.csv");
}
```

To keep this short we also added a separate class to build API request, a class to call the API and
a class to calculate taxes for our orders. We also combined our interfaces and classes into a single file as
a style choice. 

The project is coming together with all of our responsibilites separated. Next up...taking it to the next level. 