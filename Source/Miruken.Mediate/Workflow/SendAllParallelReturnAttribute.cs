namespace Miruken.Mediate.Workflow
{
    using Schedule;

    public class SendAllParallelReturnAttribute : WorkflowAttribute
    {
        public SendAllParallelReturnAttribute()
            : base(typeof(SendAllParallelReturn<,>))
        {          
        }
    }

    public class SendAllParallelReturn<TRequest, TResponse> 
        : WorkflowManyMiddleware<TRequest, TResponse>
    {
        protected override Scheduled Combine(TRequest request, object[] results)
        {
            return new Parallel
            {
                Requests = results
            };
        }
    }
}
