
What does this code do?


```
// read orders in CSV format
 var directories = Directory.GetFiles(@"C:\temp\orders\unprocessed");

foreach (string file in directories)
{
    // read the record contents
    string contents = File.ReadAllText(file);

    // convert the CSV record into an array
    string[] record = contents.Split(',');

    // calculate a total amount using a tax rate
    double totalAmount = Math.Round(double.Parse(record[2]) * 1.08, 2);

    // create a http request content object
    StringContent order = new StringContent(JsonConvert.SerializeObject(new
    {
        Amount = totalAmount,
        OrderId = record[0],
        DateSubmitted = DateTime.Parse(record[1])
    }));

    // create an http client
    HttpClient client = new HttpClient();

    // call the imaginary visa API to process the order
    HttpResponseMessage response = await client.PostAsync("http://www.visa.com/api/payment/process", order);

    // write a status message to the console
    Console.WriteLine($"Order Processed:{record[0]}");

    // move the order to a processed folder.
    File.Move(file, Path.Combine(@"C:\temp\orders\processed", Path.GetFileName(file)));
}
```

This is a simple example of a batch processing system. 

What are some issues you see with this program?

I see a few issues. 

1. There is no error handling
What happens if an error occurs? Do orders get retried? Does processing just stop until a developer fixes it?

2. This code is not maintainable and it's not testable.
We're reading from disk, calculating tax rates and calling an HTTP endpoint in the same method!
This code does work. We're engineers though and just working isn't enough. We should want more. 

3. It has configuration hard coded into the app.
What happens when file firectories and API endpoints change?

4. How can we test this?
With much difficulty. Combining responsibilities makes software very difficult to test.
You can't even change the API endpoint so it's running in production or not at all.

Let's see if we can improve this in the next iteration.
