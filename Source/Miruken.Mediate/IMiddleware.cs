namespace Miruken.Mediate
{
    using System.Threading.Tasks;
    using Callback;

    public interface IMiddleware<in TRequest, TResponse>
        : IFilter<TRequest, Task<TResponse>>
    {
    }
}
