namespace Miruken.AspNet
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;
    using Callback;
    using Context;

    public class ContextualResolver
        : ContextualScope, IDependencyResolver
    {
        private readonly IContext _context;

        public ContextualResolver(IContext context)
            : base(context)
        {
            _context = context;
        }

        public IDependencyScope BeginScope()
        {
            return new ContextualScope(_context);
        }
    }

    public class ContextualScope : IDependencyScope
    {
        private readonly IContext _context;

        public ContextualScope(IContext parent)
        {
            _context = parent.CreateChild();
        }

        public object GetService(Type serviceType)
        {
            return _context.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _context.ResolveAll(serviceType);
        }

        public void Dispose()
        {
            _context.End();
        }
    }
}
