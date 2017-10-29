namespace Miruken.Mediate.Route
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
        Inherited = false)]
    public class RoutesAttribute : Attribute, IFilterProvider
    {
        private readonly RoutesMiddleware[] _filters;

        public RoutesAttribute(params string[] schemes)
        {
            if (schemes == null || schemes.Length == 0)
                throw new ArgumentException("Schemes cannot be empty", nameof(schemes));
            _filters = new [] { new RoutesMiddleware(schemes) };
        }

        public IEnumerable<IFilter> GetFilters(MethodBinding binding, 
            Type callbackType, Type logicalResultType, IHandler composer)
        {
            return _filters;
        }

        private class RoutesMiddleware : IMiddleware<Routed, object>
        {
            private readonly string[] _schemes;

            public int? Order { get; set; } = Stage.Logging - 1;

            public RoutesMiddleware(string[] schemes)
            {
                _schemes = schemes;
            }

            public Task<object> Next(Routed routed, MethodBinding method,
                IHandler composer, NextDelegate<Task<object>> next)
            {
                var matches = Array.IndexOf(_schemes, GetScheme(routed)) >= 0;
                if (matches)
                {
                    var batch = composer.GetBatch<BatchRouter>();
                    if (batch != null)
                        return batch.Send(routed);
                }
                return next(matches);
            }

            private static string GetScheme(Routed routed)
            {
                Uri uri;
                return Uri.TryCreate(routed.Route, UriKind.Absolute, out uri)
                     ? uri.Scheme : null;
            }
        }
    }
}
