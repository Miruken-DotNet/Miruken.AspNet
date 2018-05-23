namespace Miruken.Mediate.Workflow
{
    using System;
    using Callback;

    public interface IWorkflowConfig
    {
        bool Join { get; }
    }

    public abstract class WorkflowAttribute : FilterAttribute, IWorkflowConfig
    {
        protected WorkflowAttribute(Type workflow)
            : base(workflow)
        {
        }

        public bool Join { get; set; }
    }
}
