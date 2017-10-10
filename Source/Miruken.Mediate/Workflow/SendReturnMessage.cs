﻿namespace Miruken.Mediate.Workflow
{
    using System.Threading.Tasks;
    using Callback;

    public class SendReturnMessage : WorkflowAttribute
    {
        public SendReturnMessage()
            : base(typeof(SendReturnMessage<,>))
        {          
        }
    }

    public class SendReturnMessage<TRequest, TResponse> 
        : WorkflowMiddleware<TRequest, TResponse>
    {
        protected override Task Orchestrate(TRequest request, 
            TResponse result, IHandler composer)
        {
            return composer.Send(result);
        }
    }
}
