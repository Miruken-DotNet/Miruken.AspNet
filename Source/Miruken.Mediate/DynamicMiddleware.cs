namespace Miruken.Mediate
{
    using System.Threading.Tasks;
    using Callback;

    public class DynamicMiddleware<TRequest, TResponse>
        : DynamicFilter<TRequest, Task<TResponse>>,
          IMiddleware<TRequest, TResponse>
    {
    }
}
