namespace Miruken.Mediator.Castle
{
    using System.Diagnostics;
    using Callback;
    using Callback.Policy;
    using Concurrency;
    using global::Castle.Core.Logging;

    public class LoggingMiddleware<Req, Res> : IMiddleware<Req, Res>
    {
        public int? Order { get; set; } = Stage.Logging;

        public ILoggerFactory LoggerFactory { get; set; }

        public Promise<Res> Next(Req request, MethodBinding method, 
            IHandler composer, NextDelegate<Promise<Res>> next)
        {
            var start = Stopwatch.GetTimestamp();
            return next().Then((resp, s) =>
            {
                return resp;
            });
        }

        public ILogger GetLogger(MethodBinding method)
        {
            var type = method.Dispatcher.Method.ReflectedType;
            return LoggerFactory?.Create(type) ?? NullLogger.Instance;
        }
    }
}
