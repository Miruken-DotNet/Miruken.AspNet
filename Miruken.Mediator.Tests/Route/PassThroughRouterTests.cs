namespace Miruken.Mediator.Tests.Route
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Mediator.Route;

    [TestClass]
    public class PassThroughRouterTests
    {
        [TestMethod]
        public async Task Should_Pass_Through_Requests()
        {
            var handler = new StockQuoteHandler()
                        + new RouteHandler(new PassThroughRouter());
            var quote   = await handler.Send(new GetStockQuote("AAPL").RouteTo("pass-through"));
            Assert.AreEqual("AAPL", quote.Symbol);
        }
    }
}
