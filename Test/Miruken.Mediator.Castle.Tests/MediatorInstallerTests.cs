namespace Miruken.Mediator.Castle.Tests
{
    using System.Linq;
    using global::Castle.Windsor;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;
    using Miruken.Mediator.Tests;
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
                .Install(Plugins.FromAssemblies(
                             typeof(LoggingMiddleware<,>).Assembly,
                             typeof(ValidationMiddleware<,>).Assembly,
                             typeof(Team).Assembly),
                         new ValidationInstaller(),
                         new MediatorInstaller());
            _container.Kernel.AddHandlersFilter(new ContravariantFilter());
            _handler = new WindsorHandler(_container);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _container.Dispose();
        }

        [TestMethod]
        public void Should_Register_Interface_Middleware()
        {
            var middleWare = _container.ResolveAll<IMiddleware<GetStockQuote, StockQuote>>();
            Assert.AreEqual(2, middleWare.Length);
            Assert.IsTrue(middleWare.Any(m => m is LoggingMiddleware<GetStockQuote, StockQuote>));
            Assert.IsTrue(middleWare.Any(m => m is ValidationMiddleware<GetStockQuote, StockQuote>));
        }

        [TestMethod]
        public void Should_Register_Class_Middleware()
        {
            Assert.IsNotNull(_container.Resolve<LoggingMiddleware<GetStockQuote, StockQuote>>());
            Assert.IsNotNull(_container.Resolve<ValidationMiddleware<GetStockQuote, StockQuote>>());
        }
    }
}
