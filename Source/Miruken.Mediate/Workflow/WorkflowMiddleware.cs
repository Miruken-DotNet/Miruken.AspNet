namespace Miruken.Mediate.Workflow
{
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;

    public abstract class WorkflowMiddleware<TRequest, TResponse>
        : WorkflowConfig, IMiddleware<TRequest, TResponse>
    {
        public async Task<TResponse> Next(TRequest request,
            MethodBinding method, IHandler composer,
            NextDelegate<Task<TResponse>> next)
        {
            var result = await next();
            if (result != null)
            {
                var workflow = Orchestrate(request, result, composer);
                if (workflow != null && Join)
                {
                    await workflow;
                    return result;
                }
            }
            return result;
        }

        protected abstract Task Orchestrate(TRequest request,
            TResponse result, IHandler composer);
    }

    public abstract class WorkflowManyMiddleware<TRequest, TResponse>
        : WorkflowMiddleware<TRequest, TResponse>
    {
        protected override Task Orchestrate(TRequest request,
            TResponse result, IHandler composer)
        {
            var results = (result as IEnumerable)?.Cast<object>().ToArray();
            if (results == null || results.Length == 0) return null;
            return composer.Send(Combine(request, results));
        }

        protected abstract object Combine(TRequest request, object[] results);
    }
}
