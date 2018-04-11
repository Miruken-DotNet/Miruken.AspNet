namespace Miruken.Mediate.Workflow
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using Schedule;

    public abstract class WorkflowMiddleware<TRequest, TResponse>
        : WorkflowConfig, IMiddleware<TRequest, TResponse>
    {
        public async Task<TResponse> Next(TRequest request,
            MethodBinding method, IHandler composer,
            Next<Task<TResponse>> next)
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
        protected override async Task Orchestrate(TRequest request,
            TResponse result, IHandler composer)
        {
            var results = (result as IEnumerable)?.Cast<object>().ToArray();
            if (results == null || results.Length == 0) return;
            var join = await composer.Send(Combine(request, results));
            if (Join)
            {
                var exceptions = join.Responses
                    .Where(r => r.IsError)
                    .Select(r => r.Match(ex => ex, _ => null))
                    .ToArray();
                if (exceptions.Length > 0)
                    throw new AggregateException(exceptions);
            }
        }

        protected abstract Scheduled Combine(TRequest request, object[] results);

        protected Publish[] Published(object[] results)
        {
            return results.Select(r => new Publish(r)).ToArray();
        }
    }
}
