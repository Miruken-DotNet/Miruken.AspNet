namespace Miruken.Mediate.Tests.Oneway
{
    using System.Threading.Tasks;
    using Callback.Policy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mediate.Oneway;

    [TestClass]
    public class OnewayHandlerTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            StockQuoteHandler.Called = 0;
        }

        [TestMethod]
        public async Task Should_Ignore_Response()
        {
            HandlerDescriptor.GetDescriptor<StockQuoteHandler>();
            HandlerDescriptor.GetDescriptor<OnewayHandler>();

            var handler = new StockQuoteHandler()
                        + new OnewayHandler();
            Assert.AreEqual(0, StockQuoteHandler.Called);
            var getQuote = new GetStockQuote("AAPL");
            await handler.Send(getQuote.Oneway());
            Assert.AreEqual(1, StockQuoteHandler.Called);
        }
    }
}
