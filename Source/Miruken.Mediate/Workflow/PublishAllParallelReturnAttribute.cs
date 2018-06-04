namespace Miruken.Mediate.Workflow
{
    using Schedule;

    public class PublishAllParallelReturnAttribute : WorkflowAttribute
    {
        public PublishAllParallelReturnAttribute()
            : base(typeof(PublishAllParallelReturn<,>))
        {          
        }
    }

    public class PublishAllParallelReturn<TRequest, TResponse> 
        : WorkflowManyFilter<TRequest, TResponse>
    {
        protected override Scheduled Combine(TRequest request, object[] results)
        {
            return new Parallel
            {
                Requests = Published(results)
            };
        }
    }
}
