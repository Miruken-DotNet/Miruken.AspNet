namespace Miruken.Mediator.Castle.Tests
{
    using global::Castle.Facilities.Logging;
    using global::Castle.Services.Logging.NLogIntegration;
    using global::Castle.Windsor;
    using global::Castle.Windsor.Installer;
    using NLog;
    using NLog.Config;
    using NLog.Targets;

    public class LoggingTestBase
    {
        protected MemoryTarget _MemoryTarget;
        protected IWindsorContainer _container;

        protected void InitializeContainer()
        {
            var config = new LoggingConfiguration();
            _MemoryTarget = new MemoryTarget();
            config.AddTarget("InMemoryTarget", _MemoryTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, _MemoryTarget));
            LogManager.Configuration = config;
            _container = new WindsorContainer()
                .AddFacility<LoggingFacility>(f => f.LogUsing(new NLogFactory(config)))
                .Install(FromAssembly.This()
                );
        }
    }
}
