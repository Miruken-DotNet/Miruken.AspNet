namespace Miruken.Mediator.Tests
{
    using System;
    using Callback;
    using Callback.Policy;
    using Concurrency;

    public class LogMiddleware<Cb, Res> : IMiddleware<Cb, Res>
    {
        public int? Order { get; set; }

        public Promise<Res> Next(Cb request, MethodBinding binding,
            IHandler composer, NextDelegate<Promise<Res>> next)
        {
            Console.WriteLine($"Middleware Log {request}");
            return next();
        }
    }
}
