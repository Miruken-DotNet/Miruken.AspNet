﻿namespace Miruken.Mediate.Tests.Schedule
{
    using System;
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
            HandlerDescriptor.GetDescriptor<ScheduleHandler>();
            StockQuoteHandler.Called = 0;
        }

        [TestMethod]
        public async Task Should_Execute_Concurrently()
        {
            var handler = new StockQuoteHandler()
                        + new ScheduleHandler();
            var result  = await handler.Send(new Concurrent
            {
                Requests = new[]
                {
                    new GetStockQuote("AAPL"),
                    new GetStockQuote("MSFT"),
                    new GetStockQuote("GOOGL")
                }
            });

            CollectionAssert.AreEquivalent(
                new[] {"AAPL", "MSFT", "GOOGL"},
                result.Responses.Cast<StockQuote>().Select(q => q.Symbol)
                    .ToArray());
        }

        [TestMethod]
        public async Task Should_Propogate_Single_Exception()
        {
            var handler = new StockQuoteHandler()
                        + new ScheduleHandler();
            try
            {
                await handler.Send(new Concurrent
                {
                    Requests = new[]
                    {
                        new GetStockQuote("APPL"),
                        new GetStockQuote("EX")
                    }
                });
                Assert.Fail("Expected an exception");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Stock Exchange is down", ex.Message);
            }
        }

        [TestMethod]
        public async Task Should_Propogate_Multiple_Exceptions()
        {
            var handler = new StockQuoteHandler()
                        + new ScheduleHandler();
            try
            {
                await handler.Send(new Concurrent
                {
                    Requests = new[]
                    {
                        new GetStockQuote("EX"),
                        new GetStockQuote("APPL"),
                        new GetStockQuote("EX")
                    }
                });
                Assert.Fail("Expected an exception");
            }
            catch (AggregateException ex)
            {
                Assert.AreEqual(2, ex.InnerExceptions.Count);
                Assert.AreEqual("Stock Exchange is down", ex.InnerExceptions[0].Message);
                Assert.AreEqual("Stock Exchange is down", ex.InnerExceptions[1].Message);
            }
        }

        [TestMethod]
        public async Task Should_Publish_Concurrently()
        {
            var handler = new StockQuoteHandler()
                        + new StockQuoteHandler()
                        + new ScheduleHandler();
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
