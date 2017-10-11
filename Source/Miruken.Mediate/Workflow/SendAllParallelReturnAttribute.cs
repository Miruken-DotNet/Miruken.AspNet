namespace Miruken.Mediate.Workflow
{
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
        protected override object Combine(TRequest request, object[] results)
        {
            return new Schedule.Parallel
            {
                Requests = results
            };
        }
    }
}
