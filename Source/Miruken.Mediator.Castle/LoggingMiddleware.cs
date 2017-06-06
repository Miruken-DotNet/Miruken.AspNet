namespace Miruken.Mediator.Castle
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using global::Castle.Core.Logging;

    public class LoggingMiddleware<Req, Res> : IMiddleware<Req, Res>
    {
        public int? Order { get; set; } = Stage.Logging;

        public ILoggerFactory LoggerFactory { get; set; }

        public async Task<Res> Next(Req request, MethodBinding method, 
            IHandler composer, NextDelegate<Task<Res>> next)
        {
            var start = Stopwatch.GetTimestamp();

            try
            {
                var response = await next();
                return response;
            }
            catch (Exception ex) when (LogException(method,
                GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), ex))
            {
                // Never caught, because `LogException()` returns false.
            }

            return default(Res);
        }

        private static void LogRequest(MethodBinding method, Req request)
        {
            
        }

        private static bool LogException(MethodBinding method, double elapsedMs, Exception ex)
        {
            return false;
        }

        private static double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double)Stopwatch.Frequency;
        }

        public ILogger GetLogger(MethodBinding method)
        {
            var type = method.Dispatcher.Method.ReflectedType;
            return LoggerFactory?.Create(type) ?? NullLogger.Instance;
        }
    }
}
