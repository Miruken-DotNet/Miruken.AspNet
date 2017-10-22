namespace Miruken.Mediate.Workflow
{
    using Schedule;

    public class PublishAllSequentialReturnAttribute : WorkflowAttribute
    {
        public PublishAllSequentialReturnAttribute()
            : base(typeof(PublishAllSequentialReturn<,>))
        {          
        }
    }

    public class PublishAllSequentialReturn<TRequest, TResponse> 
        : WorkflowManyMiddleware<TRequest, TResponse>
    {
        protected override Scheduled Combine(TRequest request, object[] results)
        {
            return new Sequential
            {
                Requests = Published(results)
            };
        }
    }
}
