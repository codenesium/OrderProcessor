using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace OrderProcessor.UnitTests
{
    public class OrderParserTests
    {
        [Fact]
        public void Parse_RecordIsReturned()
        {
            // Given
            OrderParser orderParser = new OrderParser();

            // When
            List<Order> orders = orderParser.Parse(new List<string>()
            {
                "123,2020-08-17,100"
            });

            // Then
            orders.Count.Should().Be(1);
            orders[0].Amount.Should().Be(100);
            orders[0].DateSubmitted.Should().Be(DateTime.Parse("2020-08-17"));
            orders[0].OrderId.Should().Be(123);
        }
    }
}