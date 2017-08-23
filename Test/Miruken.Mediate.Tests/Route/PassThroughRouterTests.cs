namespace Miruken.Mediate.Tests.Route
{
    using System.Threading.Tasks;
    using Callback.Policy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mediate.Route;

    [TestClass]
    public class PassThroughRouterTests
    {
        [TestMethod]
        public async Task Should_Pass_Through_Requests()
        {
            HandlerDescriptor.GetDescriptor<StockQuoteHandler>();
            HandlerDescriptor.GetDescriptor<PassThroughRouter>();

            var handler = new StockQuoteHandler() + new PassThroughRouter();
            var quote   = await handler.Send(new GetStockQuote("AAPL").RouteTo("pass-through"));
            Assert.AreEqual("AAPL", quote.Symbol);
        }
    }
}
