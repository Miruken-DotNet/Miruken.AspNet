namespace Miruken.Mediator
{
    using Callback;
    using Concurrency;

    public interface IMiddleware<in TRequest, TResponse>
        : IFilter<TRequest, Promise<TResponse>>
    {
    }
}
