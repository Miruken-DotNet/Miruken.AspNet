namespace Miruken.AspNet
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Callback;
    using Context;
    using Infrastructure;

    public class ContextualControllerFactory : DefaultControllerFactory
    {
        [DebuggerStepThrough]
        protected override IController GetControllerInstance(
            RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                return ControllerNotFound(requestContext);
            var context    = requestContext.GetMirukenContext();
            var controller = (IController)context.Resolve(controllerType);
            if (controller == null && !controllerType.Is<IContextual>())
                controller = base.GetControllerInstance(requestContext, controllerType);
            return controller ?? ControllerNotFound(requestContext);
        }

        public override void ReleaseController(IController controller)
        {
            if (!(controller is IContextual))
                base.ReleaseController(controller);
        }

        private static IController ControllerNotFound(RequestContext requestContext)
        {
            throw new HttpException(404,
                $"The controller for path '{requestContext.HttpContext.Request.Path}' could not be found.");
        }
    }
}
