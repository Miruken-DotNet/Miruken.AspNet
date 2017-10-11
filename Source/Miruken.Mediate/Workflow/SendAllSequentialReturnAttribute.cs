namespace Miruken.Mediate.Workflow
{
    using Schedule;

    public class SendAllSequentialReturnAttribute : WorkflowAttribute
    {
        public SendAllSequentialReturnAttribute()
            : base(typeof(SendAllSequentialReturn<,>))
        {          
        }
    }

    public class SendAllSequentialReturn<TRequest, TResponse> 
        : WorkflowManyMiddleware<TRequest, TResponse>
    {
        protected override object Combine(TRequest request, object[] results)
        {
            return new Sequential
            {
                Requests = results
            };
        }
    }
}
