namespace Miruken.Mediator.Tests.Route
{
    using System;
    using System.Threading.Tasks;
    using Callback;
    using Concurrency;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Mediator.Route;

    [TestClass]
    public class RouteHandlerTests
    {
        [TestMethod]
        public async Task Should_Route_Requests()
        {
            var handler = new StockQuoteHandler()
                        + new RouteHandler(new TrashRouter());
            var quote = await handler.Send(new GetStockQuote("GOOGL").RouteTo("Trash"));
            Assert.IsNull(quote);
        }

        [TestMethod]
        public async Task Should_Route_Requests_With_No_Responses()
        {
            var handler = new StockQuoteHandler()
                        + new RouteHandler(new TrashRouter());
            await handler.Send(new Pickup().RouteTo("Trash"));
        }

        [TestMethod]
        public async Task Should_Fail_For_Unrecognized_Routes()
        {
            var handler = new StockQuoteHandler()
                        + new RouteHandler(new TrashRouter());
            try
            {
                await handler.Send(new Pickup().RouteTo("NoWhere"));
            }
            catch (NotSupportedException ex)
            {
                Assert.AreEqual(ex.Message, "Unrecognized route 'NoWhere'");
            }
        }

        public class Pickup : IRequest
        {
        }

        public class TrashRouter : IRouter
        {
            public bool CanRoute(Routed route)
            {
                return route.Route == "Trash";
            }

            public Promise Route(Routed route, IHandler composer)
            {
                return Promise.Empty;
            }
        }
    }
}
