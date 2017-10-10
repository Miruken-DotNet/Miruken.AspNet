namespace Miruken.Mediate
{
    using System.Threading.Tasks;
    using Callback;

    public class DynamicMiddleware<TRequest, TResponse>
        : DynamicFilter<TRequest, Task<TResponse>>,
          IMiddleware<TRequest, TResponse>
    {
    }

    public class DynamicGlobalMiddleware<TRequest, TResponse>
        : DynamicMiddleware<TRequest, TResponse>,
          IGlobalMiddleware<TRequest, TResponse>
    {
    }
}
