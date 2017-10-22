﻿namespace Miruken.Mediate.Tests.Schedule
{
    using System.Linq;
    using System.Threading.Tasks;
    using Callback.Policy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mediate.Schedule;

    [TestClass]
    public class ConcurrentTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.GetDescriptor<StockQuoteHandler>();
            HandlerDescriptor.GetDescriptor<Scheduler>();
            StockQuoteHandler.Called = 0;
        }

        [TestMethod]
        public async Task Should_Execute_Concurrently()
        {
            var handler = new StockQuoteHandler()
                        + new Scheduler();
            var result  = await handler.Send(new Concurrent
            {
                Requests = new[]
                {
                    new GetStockQuote("APPL"),
                    new GetStockQuote("MSFT"),
                    new GetStockQuote("GOOGL")
                }
            });
            CollectionAssert.AreEqual(
                new[] {"APPL", "MSFT", "GOOGL"},
                result.Responses.Select(r => r.Match(
                        error => error.Message,
                        quote => ((StockQuote)quote).Symbol))
                    .ToArray());
        }

        [TestMethod]
        public async Task Should_Propogate_Single_Exception()
        {
            var handler = new StockQuoteHandler()
                        + new Scheduler();
            var result  = await handler.Send(new Concurrent
            {
                Requests = new[]
                {
                    new GetStockQuote("APPL"),
                    new GetStockQuote("EX")
                }
            });
            CollectionAssert.AreEqual(
                new[] { "APPL", "Stock Exchange is down" },
                result.Responses.Select(r => r.Match(
                    error => error.Message,
                    quote => ((StockQuote)quote).Symbol))
                    .ToArray());
        }

        [TestMethod]
        public async Task Should_Propogate_Multiple_Exceptions()
        {
            var handler = new StockQuoteHandler()
                        + new Scheduler();
            var result  = await handler.Send(new Concurrent
            {
                Requests = new[]
                {
                    new GetStockQuote("EX"),
                    new GetStockQuote("APPL"),
                    new GetStockQuote("EX")
                }
            });
            CollectionAssert.AreEqual(
                new[] { "Stock Exchange is down" , "APPL", "Stock Exchange is down" },
                result.Responses.Select(r => r.Match(
                        error => error.Message,
                        quote => ((StockQuote)quote).Symbol))
                    .ToArray());
        }

        [TestMethod]
        public async Task Should_Publish_Concurrently()
        {
            var handler = new StockQuoteHandler()
                        + new StockQuoteHandler()
                        + new Scheduler();
            var result = await handler.Send(new Concurrent
            {
                Requests = new[]
                {
                    new SellStock("AAPL", 2).Publish(),
                    new SellStock("MSFT", 1).Publish(),
                    new SellStock("GOOGL", 2).Publish()
                }
            });

            Assert.AreEqual(3, result.Responses.Length);
            Assert.AreEqual(6, StockQuoteHandler.Called);
        }
    }
}
