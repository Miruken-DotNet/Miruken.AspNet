namespace Miruken.Mediator.Tests
{
    using System;
    using Callback;
    using Callback.Policy;
    using Concurrency;

    public class LogBehavior<Cb, Res> : IMiddleware<Cb, Res>
    {
        public int? Order { get; set; }

        public Promise<Res> Filter(Cb callback, MethodBinding binding,
            IHandler composer, FilterDelegate<Promise<Res>> next)
        {
            Console.WriteLine($"Handle {callback}");
            return next();
        }
    }
}
