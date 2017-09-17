﻿namespace Miruken.AspNet
{
    using System.Web.Http;
    using Context;

    public abstract class ContextualApiController : ApiController, IContextual
    {
        private IContext _context;

        public IContext Context
        {
            get { return _context; }
            set
            {
                if (_context == value) return;
                var newContext = value;
                ContextChanging?.Invoke(this, _context, ref newContext);
                _context?.RemoveHandlers(this);
                var oldContext = _context;
                _context = newContext;
                _context?.InsertHandlers(0, this);
                ContextChanged?.Invoke(this, oldContext, _context);
            }
        }

        public event ContextChangingDelegate<IContext> ContextChanging;
        public event ContextChangedDelegate<IContext> ContextChanged;
    }
}