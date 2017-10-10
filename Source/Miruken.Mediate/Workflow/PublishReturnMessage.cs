namespace Miruken.Mediate.Workflow
{
    using System.Threading.Tasks;
    using Callback;

    public class PublishReturnMessage : WorkflowAttribute
    {
        public PublishReturnMessage()
            : base(typeof(PublishReturnMessage<,>))
        {          
        }
    }

    public class PublishReturnMessage<TRequest, TResponse> 
        : WorkflowMiddleware<TRequest, TResponse>
    {
        protected override Task Orchestrate(TRequest request, 
            TResponse result, IHandler composer)
        {
            return composer.Publish(result);
        }
    }
}
