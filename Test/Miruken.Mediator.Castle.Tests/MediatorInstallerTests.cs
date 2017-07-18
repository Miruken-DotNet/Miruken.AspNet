namespace Miruken.Mediator.Castle.Tests
{
    using System;
    using System.Linq;
    using global::Castle.MicroKernel;
    using global::Castle.Windsor;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;
    using Mediator.Tests;
    using Validate.Castle;

    [TestClass]
    public class MediatorInstallerTests
    {
        protected IWindsorContainer _container;
        protected WindsorHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            _container = new WindsorContainer()
                .Install(WithFeatures.FromAssemblies(typeof(Team).Assembly),
                         new ValidationInstaller(),
                         new MediatorInstaller().WithMiddleware());
            _container.Kernel.AddHandlersFilter(new ContravariantFilter());
            _handler = new WindsorHandler(_container);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _container.Dispose();
        }

        [TestMethod]
        public void Should_Register_Middleware_By_Interface()
        {
            var middleWare = _container.ResolveAll<IMiddleware<GetStockQuote, StockQuote>>();
            Assert.AreEqual(2, middleWare.Length);
            Assert.IsTrue(middleWare.Any(m => m is LoggingMiddleware<GetStockQuote, StockQuote>));
            Assert.IsTrue(middleWare.Any(m => m is ValidationMiddleware<GetStockQuote, StockQuote>));
        }

        [TestMethod]
        public void Should_Register_Middleware_By_Class()
        {
            Assert.IsNotNull(_container.Resolve<LoggingMiddleware<GetStockQuote, StockQuote>>());
            Assert.IsNotNull(_container.Resolve<ValidationMiddleware<GetStockQuote, StockQuote>>());
        }

        [TestMethod,
         ExpectedException(typeof(ComponentNotFoundException))]
        public void Should_Not_Install_Middleware_By_Default()
        {
            var container = new WindsorContainer().Install(new MediatorInstaller());
            container.Resolve<LoggingMiddleware<string, int>>();
        }

        [TestMethod]
        public void Should_Install_Specific_Middleware()
        {
            var container = new WindsorContainer()
                .Install(new MediatorInstaller().WithMiddleware(typeof(LoggingMiddleware<,>)));
            var logging = container.Resolve<LoggingMiddleware<string, int>>();
            Assert.IsNotNull(logging);
        }

        [TestMethod,
         ExpectedException(typeof(ArgumentException))]
        public void Should_Reject_Invalid_Middleware()
        {
             new WindsorContainer()
                .Install(new MediatorInstaller().WithMiddleware(typeof(MediatorInstallerTests)));
        }
    }
}
