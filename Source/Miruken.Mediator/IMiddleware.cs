namespace Miruken.Mediator
{
    using System.Threading.Tasks;
    using Callback;

    public interface IMiddleware<in TRequest, TResponse>
        : IFilter<TRequest, Task<TResponse>>
    {
    }
}
