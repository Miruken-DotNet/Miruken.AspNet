namespace Miruken.Mediate.Workflow
{
    using System.Linq;
    using Schedule;

    public class PublishAllParallelReturnAttribute : WorkflowAttribute
    {
        public PublishAllParallelReturnAttribute()
            : base(typeof(PublishAllParallelReturn<,>))
        {          
        }
    }

    public class PublishAllParallelReturn<TRequest, TResponse> 
        : WorkflowManyMiddleware<TRequest, TResponse>
    {
        protected override object Combine(TRequest request, object[] results)
        {
            return new Parallel
            {
                Requests = results.Select(r => new Publish(r)).ToArray()
            };
        }
    }
}
