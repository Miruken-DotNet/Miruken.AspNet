namespace Miruken.Mediator.Castle.Tests
{
    using global::Castle.Facilities.Logging;
    using global::Castle.Services.Logging.NLogIntegration;
    using global::Castle.Windsor;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NLog;
    using NLog.Config;
    using NLog.Targets;

    [TestClass]
    public class LoggingMiddlewareTests
    {
        protected MemoryTarget _memoryTarget;
        protected IWindsorContainer _container;

        [TestInitialize]
        public void TestInitialize()
        {
            var config = new LoggingConfiguration();
            _memoryTarget = new MemoryTarget();
            config.AddTarget("InMemoryTarget", _memoryTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, _memoryTarget));
            LogManager.Configuration = config;
            _container = new WindsorContainer()
                .AddFacility<LoggingFacility>(f => f.LogUsing(new NLogFactory(config))
                );
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _container.Dispose();
        }
    }
}
