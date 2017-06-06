namespace Miruken.Mediator.Tests
{
    using System;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;

    public class LogMiddleware<Cb, Res> : IMiddleware<Cb, Res>
    {
        public int? Order { get; set; }

        public async Task<Res> Next(Cb request, MethodBinding binding,
            IHandler composer, NextDelegate<Task<Res>> next)
        {
            Console.WriteLine($"Middleware Log {request}");
            var response = await next();
            return response;
        }
    }
}
