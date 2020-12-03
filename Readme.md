This project is a learning exercise for how to take untestable code and transform it
into something mantainable and tested.

This app processes a CSV list of orders and calls the imaginary Visa processing API.

To run create two directories C:\temp\orders\unprocessed and C:\temp\orders\processed and copy the orders.csv
to C:\temp\orders\unprocessed.


OrderProcessor1 is our baseline. It works but there's no way to test it and it doesn't handle errors well.

OrderProcessor2 cleans up the project and breaks it into responsibilities.

OrderProcessor3 takes it to the next level by adding tests. 

Have fun and please submit PRs or comments for how we can improve this project. 