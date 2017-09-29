namespace Miruken.AspNet
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Callback;
    using Context;

    public class ContextualControllerFactory : DefaultControllerFactory
    {
        private readonly IContext _context;

        public ContextualControllerFactory(IContext context)
        {
            _context = context;
        }

        [DebuggerStepThrough]
        protected override IController GetControllerInstance(
            RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404,
                    $"The controller for path '{requestContext.HttpContext.Request.Path}' could not be found.");
            }
            return (IController)_context.Resolve(controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            // Will be released when context ends
        }
    }
}
