namespace Miruken.Mediate.Workflow
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using Schedule;

    public abstract class WorkflowFilter<TRequest, TResponse>
        : IFilter<TRequest, TResponse>
    {
        public int? Order { get; set; }

        public async Task<TResponse> Next(TRequest request,
            MemberBinding method, IHandler composer,
            Next<TResponse> next, IFilterProvider provider)
        {
            var result = await next();
            if (result != null)
            {
                var config   = provider as IWorkflowConfig;
                var workflow = Orchestrate(request, result,
                    composer.SkipFilters(false), config);
                if (workflow != null && config?.Join == true)
                {
                    await workflow;
                    return result;
                }
            }
            return result;
        }

        protected abstract Task Orchestrate(TRequest request,
            TResponse result, IHandler composer, IWorkflowConfig config);
    }

    public abstract class WorkflowManyFilter<TRequest, TResponse>
        : WorkflowFilter<TRequest, TResponse>
    {
        protected override async Task Orchestrate(TRequest request,
            TResponse result, IHandler composer, IWorkflowConfig config)
        {
            var results = (result as IEnumerable)?.Cast<object>().ToArray();
            if (results == null || results.Length == 0) return;
            var join = await composer.Send(Combine(request, results));
            if (config?.Join == true)
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
