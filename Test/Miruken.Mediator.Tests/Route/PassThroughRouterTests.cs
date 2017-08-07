namespace Miruken.Mediator.Tests.Route
{
    using System.Threading.Tasks;
    using Callback.Policy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mediator.Route;

    [TestClass]
    public class PassThroughRouterTests
    {
        [TestMethod]
        public async Task Should_Pass_Through_Requests()
        {
            HandlerDescriptor.GetDescriptor<StockQuoteHandler>();
            HandlerDescriptor.GetDescriptor<RouteHandler>();

            var handler = new StockQuoteHandler()
                        + new RouteHandler(new PassThroughRouter());
            var quote   = await handler.Send(new GetStockQuote("AAPL").RouteTo("pass-through"));
            Assert.AreEqual("AAPL", quote.Symbol);
        }
    }
}
