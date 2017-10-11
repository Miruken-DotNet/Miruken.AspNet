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
        : WorkflowManyMiddleware<TRequest, TResponse>
    {
        protected override object Combine(TRequest request, object[] results)
        {
            return new Concurrent
            {
                Requests = results
            };
        }
    }
}
