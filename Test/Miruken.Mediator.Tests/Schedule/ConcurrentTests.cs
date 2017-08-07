namespace Miruken.Mediator.Tests.Schedule
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback.Policy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mediator.Schedule;

    [TestClass]
    public class ConcurrentTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.GetDescriptor<StockQuoteHandler>();
            HandlerDescriptor.GetDescriptor<ScheduleHandler>();
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
        public async Task Should_Propogate_Exception()
        {
            var handler = new StockQuoteHandler()
                        + new ScheduleHandler();
            try
            {
                await handler.Send(new Concurrent
                {
                    Requests = new[]
                    {
                        new GetStockQuote("AAPL"),
                        new GetStockQuote("EX"),
                        new GetStockQuote("GOOGL")
                    }
                });
                Assert.Fail("Expected an exception");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Stock Exchange is down", ex.Message);
            }
        }
    }
}
