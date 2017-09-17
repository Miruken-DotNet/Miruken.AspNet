namespace Miruken.Mediate.Tests.Route
{
    using System;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using Concurrency;
    using FluentValidation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mediate.Route;
    using Validate;
    using Validate.DataAnnotations;
    using Validate.FluentValidation;

    [TestClass]
    public class RouteTests
    {
        private IHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.GetDescriptor<StockQuoteHandler>();
            HandlerDescriptor.GetDescriptor<PassThroughRouter>();
            HandlerDescriptor.GetDescriptor<TrashHandler>();

            _handler = new StockQuoteHandler()
                     + new PassThroughRouter()
                     + new TrashHandler()
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

        [TestMethod,
         ExpectedException(typeof(NotSupportedException))]
        public async Task Should_Fail_For_Unrecognized_Routes()
        {
            var handler = new StockQuoteHandler()
                        + new PassThroughRouter();
            await handler.Send(new Pickup().RouteTo("NoWhere"));
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

        public class TrashHandler : Handler
        {
            public const string Scheme = "Trash";

            [Mediates]
            public Promise Route(Routed request, IHandler composer)
            {
                return request.Route == Scheme ? Promise.Empty : null;
            }
        }

        private class MiddlewareProvider : Handler
        {
            [Provides]
            public IMiddleware<TReq, TResp>[] GetMiddleware<TReq, TResp>()
            {
                return new IMiddleware<TReq, TResp>[]
                {
                    new ValidateMiddleware<TReq, TResp>()
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
