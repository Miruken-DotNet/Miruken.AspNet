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
        : ContextualScope, IDependencyResolver,
          IHttpControllerActivator
    {
        private readonly IHttpControllerActivator _defaultActivator;

        public ContextualResolver(
            IContext parent,
            IHttpControllerActivator defaultActivator)
            : base(parent)
        {
            _defaultActivator = defaultActivator;
        }

        IHttpController IHttpControllerActivator.Create(
            HttpRequestMessage request,
            HttpControllerDescriptor controllerDescriptor,
            Type controllerType)
        {
            if (controllerType.Is<IContextual>())
            {
                var scope = (ContextualScope)request.GetDependencyScope();
                scope.AdjustScope(request);
                return (IHttpController)scope.GetService(controllerType);
            }
            return _defaultActivator?.Create(
                request, controllerDescriptor, controllerType);
        }
    }

    public class ContextualScope : IDependencyScope
    {
        protected IContext Context;
        private readonly object _guard = new object();

        public ContextualScope(IContext parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            lock (_guard)
            {
                Context = parent.CreateChild();
            }
        }

        public IDependencyScope BeginScope()
        {
            return new ContextualScope(Context);
        }

        public object GetService(Type serviceType)
        {
            return serviceType != typeof(IHttpControllerActivator)
                 ? Context.Resolve(serviceType)
                 : this;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Context.ResolveAll(serviceType);
        } 

        internal void AdjustScope(HttpRequestMessage request)
        {
            var context = Context.BestEffort()
                .Proxy<ILogicalContextSelector>()
                .SelectApiContext(request);
            if (context != null && context != Context.Parent)
            {
                lock (_guard)
                {
                    Context.End();
                    Context = context.CreateChild();
                }
            }
        }

        public void Dispose()
        {
            lock (_guard)
            {
                Context.End();
            }
        }
    }
}
