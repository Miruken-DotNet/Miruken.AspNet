namespace Miruken.Mediate.Workflow
{
    using Schedule;

    public class SendAllReturnAttribute : WorkflowAttribute
    {
        public SendAllReturnAttribute()
            : base(typeof(SendAllReturn<,>))
        {          
        }
    }

    public class SendAllReturn<TRequest, TResponse> 
        : WorkflowManyFilter<TRequest, TResponse>
    {
        protected override Scheduled Combine(TRequest request, object[] results)
        {
            return new Concurrent
            {
                Requests = results
            };
        }
    }
}
