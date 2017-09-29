namespace Miruken.AspNet
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dependencies;
    using System.Web.Http.Dispatcher;
    using Callback;
    using Context;

    public class ContextualResolver
        : ContextualScope, IDependencyResolver,
          IHttpControllerActivator
    {
        public ContextualResolver(IContext context)
            : base(context)
        {
            context.AddHandlers(new Provider(this));
        }

        public IDependencyScope BeginScope()
        {
            return new ContextualScope(_context);
        }

        IHttpController IHttpControllerActivator.Create(
            HttpRequestMessage request,
            HttpControllerDescriptor controllerDescriptor,
            Type controllerType)
        {
            var scope = (ContextualScope)request.GetDependencyScope();
            scope.SelectClientContext(request);
            return (IHttpController)scope.GetService(controllerType);
        }
    }

    public class ContextualScope : IDependencyScope
    {
        protected IContext _context;

        public ContextualScope(IContext context)
        {
            _context = context;
        }

        public object GetService(Type serviceType)
        {
            return _context.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _context.ResolveAll(serviceType);
        }

        internal void SelectClientContext(HttpRequestMessage request)
        {
            _context = _context.BestEffort()
                           .Proxy<IClientContextSelector>()
                           .SelectContext(request)
                    ?? _context.CreateChild();
        }

        public void Dispose()
        {
            _context.End();
        }
    }
}
