namespace Miruken.Mediate.Workflow
{
    using System;
    using Callback;
    using Callback.Policy;

    public class WorkflowConfig
    {
        public int? Order { get; set; }
        public bool Join  { get; internal set; }
    }

    public abstract class WorkflowAttribute : FilterAttribute
    {
        protected WorkflowAttribute(Type workflow)
            : base(workflow)
        {
        }

        public bool Join { get; set; }

        protected override bool UseFilterInstance(
            IFilter filter, MethodBinding binding)
        {
            var send = (WorkflowConfig)filter;
            send.Join = Join;
            return true;
        }
    }
}
