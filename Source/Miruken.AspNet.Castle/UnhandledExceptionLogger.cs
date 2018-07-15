namespace Miruken.AspNet.Castle
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.ExceptionHandling;
    using global::Castle.Core.Logging;
    using Miruken;

    public class UnhandledExceptionLogger : IExceptionLogger
    {
        private readonly ILogger _logger;

        public UnhandledExceptionLogger(ILogger logger)
        {
            _logger = logger;
        }

        public Task LogAsync(ExceptionLoggerContext context,
            CancellationToken cancellationToken)
        {
            if (!Equals(context.Exception?.Data[Stage.Logging], true))
                _logger.Error("Unhandled exception", context.Exception);
            return Task.FromResult(true);
        }
    }
}
