namespace Miruken.Mediate
{
    using System.Threading.Tasks;
    using Callback;

    public interface IMiddleware<in TRequest>
        : IMiddleware<TRequest, object>
    {
    }

    public interface IMiddleware<in TRequest, TResponse>
        : IFilter<TRequest, Task<TResponse>>
    {
    }

    public interface IGlobalMiddleware<in TRequest>
        : IGlobalMiddleware<TRequest, object>
    {
    }

    public interface IGlobalMiddleware<in TRequest, TResponse>
        : IMiddleware<TRequest, TResponse>,
          IGlobalFilter<TRequest, Task<TResponse>>
    {
    }
}
