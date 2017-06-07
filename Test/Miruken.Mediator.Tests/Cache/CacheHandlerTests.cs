﻿namespace Miruken.Mediator.Tests.Cache
{
    using System;
    using System.Threading.Tasks;
    using Callback;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Mediator.Cache;
    using Miruken.Mediator.Middleware;
    using Validate;
    using Validate.DataAnnotations;
    using Validate.FluentValidation;

    [TestClass]
    public class CacheHandlerTests
    {
        private IHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            _handler = new StockQuoteHandler()
                     + new CachedHandler()
                     + new MiddlewareProvider();

            StockQuoteHandler.Called = 0;
        }

        [TestMethod]
        public async Task Should_Make_Initial_Request()
        {
            Assert.AreEqual(0, StockQuoteHandler.Called);
            var getQuote = new GetStockQuote("AAPL");
            var quote    = await _handler.Send(getQuote.Cached());
            Assert.IsNotNull(quote);
            Assert.AreEqual(1, StockQuoteHandler.Called);
        }

        [TestMethod]
        public async Task Should_Cache_Initial_Response()
        {
            Assert.AreEqual(0, StockQuoteHandler.Called);
            var getQuote = new GetStockQuote("AAPL");
            var quote1   = await _handler.Send(getQuote.Cached());
            var quote2   = await _handler.Send(getQuote.Cached());
            Assert.AreEqual(1, StockQuoteHandler.Called);
            Assert.AreEqual(quote1.Value, quote2.Value);
        }

        [TestMethod]
        public async Task Should_Refresh_Response()
        {
            Assert.AreEqual(0, StockQuoteHandler.Called);
            var getQuote = new GetStockQuote("AAPL");
            var quote1 = await _handler.Send(getQuote.Cached());
            var quote2 = await _handler.Send(getQuote.Cached());
            var quote3 = await _handler.Send(getQuote.Refresh());
            Assert.AreEqual(2, StockQuoteHandler.Called);
            Assert.AreEqual(quote1.Value, quote2.Value);
            Assert.AreEqual(quote1.Symbol, quote3.Symbol);
        }

        [TestMethod]
        public async Task Should_Refresh_Stale_Response()
        {
            Assert.AreEqual(0, StockQuoteHandler.Called);
            var getQuote = new GetStockQuote("AAPL");
            await _handler.Send(getQuote.Cached());
            await Task.Delay(TimeSpan.FromSeconds(.2));
            await _handler.Send(getQuote.Cached(TimeSpan.FromSeconds(.1)));
            Assert.AreEqual(2, StockQuoteHandler.Called);
        }

        [TestMethod]
        public async Task Should_Invalidate_Cache()
        {
            Assert.AreEqual(0, StockQuoteHandler.Called);
            var getQuote = new GetStockQuote("AAPL");
            var quote1   = await _handler.Send(getQuote.Cached());
            var quote2   = await _handler.Send(getQuote.Cached());
            var quote3   = await _handler.Send(getQuote.Invalidate());
            var quote4   = await _handler.Send(getQuote.Cached());
            Assert.AreEqual(2, StockQuoteHandler.Called);
            Assert.AreEqual(quote1.Value, quote2.Value);
            Assert.AreEqual(quote1.Value, quote3.Value);
            Assert.AreEqual(quote1.Value, quote3.Value);
            Assert.IsNotNull(quote4);
        }

        [TestMethod]
        public async Task Should_Not_Cache_Exceptions()
        {
            Assert.AreEqual(0, StockQuoteHandler.Called);
            var getQuote = new GetStockQuote("EX");
            try
            {
                await _handler.Send(getQuote.Cached());
                Assert.Fail("Expected exception");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Stock Exchange is down", ex.Message);
            }
            try
            {
                await _handler.Send(getQuote.Cached());
                Assert.Fail("Expected exception");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Stock Exchange is down", ex.Message);
            }
            Assert.AreEqual(2, StockQuoteHandler.Called);
        }

        private class MiddlewareProvider : Handler
        {
            [Provides]
            public IMiddleware<TReq, TResp>[] GetMiddleware<TReq, TResp>()
            {
                return new IMiddleware<TReq, TResp>[]
                {
                    new LogMiddleware<TReq, TResp>()
                };
            }
        }
    }
}
