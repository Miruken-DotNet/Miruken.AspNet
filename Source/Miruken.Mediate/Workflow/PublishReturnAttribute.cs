namespace Miruken.Mediate.Workflow
{
    using System.Threading.Tasks;
    using Callback;

    public class PublishReturnAttribute : WorkflowAttribute
    {
        public PublishReturnAttribute()
            : base(typeof(PublishReturn<,>))
        {          
        }
    }

    public class PublishReturn<TRequest, TResponse> 
        : WorkflowMiddleware<TRequest, TResponse>
    {
        protected override Task Orchestrate(TRequest request, 
            TResponse result, IHandler composer, IWorkflowConfig config)
        {
            return composer.Publish(result);
        }
    }
}
