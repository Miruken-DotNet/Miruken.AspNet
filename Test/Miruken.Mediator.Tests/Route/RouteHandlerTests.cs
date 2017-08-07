namespace Miruken.Mediator.Tests.Route
{
    using System;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using Concurrency;
    using FluentValidation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mediator.Route;
    using Validate;
    using Validate.DataAnnotations;
    using Validate.FluentValidation;

    [TestClass]
    public class RouteHandlerTests
    {
        private IHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.GetDescriptor<StockQuoteHandler>();
            HandlerDescriptor.GetDescriptor<RouteHandler>();

            _handler = new StockQuoteHandler()
                     + new RouteHandler(new TrashRouter(), new PassThroughRouter())
                     + new MiddlewareProvider()
                     + new DataAnnotationsValidator()
                     + new FluentValidationValidator()
                     + new ValidationHandler();
            StockQuoteHandler.Called = 0;
        }

        [TestMethod]
        public async Task Should_Route_Requests()
        {
            var quote = await _handler.Send(new GetStockQuote("GOOGL").RouteTo("Trash"));
            Assert.IsNull(quote);
        }

        [TestMethod]
        public async Task Should_Route_Requests_With_No_Responses()
        {
            await _handler.Send(new Pickup().RouteTo("Trash"));
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

        [TestMethod]
        public async Task Should_Route_Request_Through_Pipeline()
        {
            var quote = await _handler.Send(new GetStockQuote("MSFT")
                .RouteTo(PassThroughRouter.Scheme));
            Assert.AreSame("MSFT", quote.Symbol);
        }

        [TestMethod]
        public async Task Should_Validate_Routed_Requests()
        {
            try
            {
                await _handler.Send(new GetStockQuote()
                    .RouteTo(PassThroughRouter.Scheme));
            }
            catch (Validate.ValidationException vex)
            {
                var outcome = vex.Outcome;
                Assert.IsNotNull(outcome);
                CollectionAssert.AreEqual(new[] { "Symbol" }, outcome.Culprits);
                Assert.AreEqual("'Symbol' should not be empty.", outcome["Symbol"]);
            }
        }

        public class Pickup
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

        private class MiddlewareProvider : Handler
        {
            [Provides]
            public IMiddleware<TReq, TResp>[] GetMiddleware<TReq, TResp>()
            {
                return new IMiddleware<TReq, TResp>[]
                {
                    new ValidationMiddleware<TReq, TResp>()
                };
            }

            [Provides]
            public IValidator<GetStockQuote>[] StockQuoteValidators()
            {
                return new[] { new GetStockQuoteIntegrity(),  };
            }
        }
    }
}
