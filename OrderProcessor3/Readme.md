In this iteration we're cleaning things up and adding tests.


## HttpClient
Instead of using HttpClient like this
```
HttpClient client = new HttpClient();
```
It's better to HttpClinetFactory by calling this in startup
```
serviceCollection.AddHttpClient();
```
And in your class using HttpClient inject IHttpClientFactory and where you need an HttpClient use
```
HttpClient client = this.httpClientFactory.CreateClient();

```

This will keep your app from exaustin ports and is the recommended way to create HttpClient.
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0


## Configuration

Configuration is pretty simple in .NET 


In your entry method
```
IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
```

Then  where you register your services 
```
Settings apiSettings = configuration.GetSection("Settings").Get<Settings>();
```

Then in your classes
```
    public class ApiClient : IApiClient
    {

        private readonly Settings settings;
        public ApiClient(Settings settings)
        {
            this.settings = settings;
        }
```

There is another way to do this using the IOptions pattern but I usually end up doing it this way.

## Resilience
Resilience is being able to handle faults in a smooth fashion. Calling an API may fail. Writing to disk may fail.

Polly is a library that helps with this problem. It can be configure to try an operation in many different ways.
For the sake of simplicity we are going to add 3 retries to our API call.

```
private async Task CallApi(StringContent request)
{
    AsyncPolicy policy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(3)
        });

    Func<Task> callApi = async () =>
    {
        HttpClient client = this.httpClientFactory.CreateClient();
        HttpResponseMessage response = await client.PostAsync(this.settings.ApiEndpoint, request);
        response.EnsureSuccessStatusCode();
    };

    await policy.ExecuteAsync(() => callApi());
}
```


# Tests

Tests give you confidence that your code works. Then makes upgrading pacakges a breeze.
They make refactoring possible.

## Unit Tests
Unit tests test code as it exists in your application. If it touches the file system, network, databases
or anything else external to your app it's an integration test. We can mock these external dependencies
to make testing possible. 

You do not have to unit test everything! Test the business logic. Test the places where
bugs will occur. Don't write pointless tests!

The test below builds an API request. It needs to calculate the order total using an IOrderTaxCalculator
object. Unit tests typically have a setup, act and teardown section. I'm using Given. When. Then. It's also common to
use Arrange. Assert. Act. The words aren't important but I do recommend denoting the sections.

In the given section we use a mock. A mock is a representation of an object. It looks like your object but
it's not. You use mocks or stubs to replace functionality. In this case we want to verify our request and
we want to say explicily what the total should be. So to do this we mock the CalculateTotalWithTax method
and say regardless of what is passed return 108.25. You can use this same strategy to mock
calls to the database, network, file system or anything else.

In the When section we make our call to create a request object.

In the Then section we verify the respone was correct. We're using Fluent Assertions here which replaces 
Assert.Equals syntax with a nice fluent interface. 

```
[Fact]
public async Task BuildRequest_AmountHasTaxAddedAndDataMatchesOrderObject()
{
    // Given
    Mock<IOrderTaxCalculator> orderTaxCalculatorMock = new Mock<IOrderTaxCalculator>();
    orderTaxCalculatorMock.Setup(x => x.CalculateTotalWithTax(It.IsAny<double>())).Returns(108.25);
    ApiOrderRequestBuilder apiOrderRequestBuilder = new ApiOrderRequestBuilder(orderTaxCalculatorMock.Object);

    // When
    StringContent content = apiOrderRequestBuilder.BuildRequest(new Order(100, DateTime.Parse("2020-08-17"), 1));

    // Then
    string result = await content.ReadAsStringAsync();
    Order order = JsonConvert.DeserializeObject<Order>(result);
    order.Amount.Should().Be(108.25);
    order.DateSubmitted.Should().Be(DateTime.Parse("2020-08-17"));
    order.OrderId.Should().Be(1);
}
```

## Integration Tests
Integration tests touch the network or the file system. Then are more brittle.
They are slow. They give you the most bang for your buck though. You should write them
first. 

In OrderProcessorTests we have an end to end test that processes files and does everything
but call a real API. In the real world you hopefully have a test server to call that you
can add to your test. I don't have a lot to say about integration tests. They are often dirty and
hacked together. The best advice I can give is make them easy to run on different machines. If possible
make it so your tests can run in docker so you know they run everwhere. 


# SOLID principles
It's on purpose that I didn't talk about the SOLID principles and hopefully by getting to
this point you picked up on them by osmosis. Breaking code into methods that have a single purpose is the 
Single Responsibility principle. Using dependency injection to compose functionality is the Dependency Inversion principle.
Keeping your classes and interfaces small is the Interface Segregation principle. We aren't discussing 
the Open/Closed and Liskov Substitution principle in this project but they are worth exploring too. 
These principes are the guide for writing testable software.


# In Conclusion
I hope you were able to gain some knowledge about how to convert non testable code into something
better. Please contribute or ask questions on the repo and I will try to answer.