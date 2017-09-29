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
    using Infrastructure;

    public class ContextualResolver
        : IDependencyResolver, IHttpControllerActivator
    {
        private readonly IContext _context;
        private readonly IHttpControllerActivator _defaultActivator;

        public ContextualResolver(
            IContext context,
            IHttpControllerActivator defaultActivator)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            _context          = context;
            _defaultActivator = defaultActivator;
        }

        public IDependencyScope BeginScope()
        {
            return new ContextualScope(_context);
        }

        public object GetService(Type serviceType)
        {
            return serviceType != typeof(IHttpControllerActivator)
                 ? _context.Resolve(serviceType)
                 : this;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _context.ResolveAll(serviceType);
        }

        IHttpController IHttpControllerActivator.Create(
            HttpRequestMessage request,
            HttpControllerDescriptor controllerDescriptor,
            Type controllerType)
        {
            if (controllerType.Is<IContextual>())
            {
                var scope = (ContextualScope) request.GetDependencyScope();
                scope.ConfigureScope(request);
                return (IHttpController)scope.GetService(controllerType);
            }
            return _defaultActivator?.Create(
                request, controllerDescriptor, controllerType);
        }

        public void Dispose()
        {
            // Don't own root context
        }
    }

    public class ContextualScope : IDependencyScope
    {
        protected readonly IContext _context;
        private IContext _clientContext;

        public ContextualScope(IContext context)
        {
            _context = context;
        }

        public object GetService(Type serviceType)
        {
            return GetClientContext().Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return GetClientContext().ResolveAll(serviceType);
        }

        internal void ConfigureScope(HttpRequestMessage request)
        {
            _clientContext = _context.BestEffort()
                                 .Proxy<IHttpContextSelector>()
                                 .SelectApiContext(request)
                          ?? _context.CreateChild();
        }

        private IContext GetClientContext()
        {
            return _clientContext ?? _context;
        }

        public void Dispose()
        {
            _clientContext?.End();
        }
    }
}
