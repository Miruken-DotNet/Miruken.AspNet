﻿namespace Miruken.Mediate.Route
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
        private readonly RoutesFilter[] _filters;

        public RoutesAttribute(params string[] schemes)
        {
            if (schemes == null || schemes.Length == 0)
                throw new ArgumentException("Schemes cannot be empty", nameof(schemes));
            _filters = new [] { new RoutesFilter(schemes) };
        }

        public IEnumerable<IFilter> GetFilters(MethodBinding binding, 
            Type callbackType, Type logicalResultType, IHandler composer)
        {
            return _filters;
        }

        private class RoutesFilter : IFilter<Routed, object>
        {
            private readonly string[] _schemes;

            public int? Order { get; set; } = Stage.Logging - 1;

            public RoutesFilter(string[] schemes)
            {
                _schemes = schemes;
            }

            public Task<object> Next(Routed routed, MethodBinding method,
                IHandler composer, Next<object> next,
                IFilterProvider provider)
            {
                var matches = Array.IndexOf(_schemes, GetScheme(routed)) >= 0;
                if (matches)
                {
                    var batch = composer
                        .GetBatch<BatchRouter>();
                    if (batch != null)
                        return batch.SkipFilters(false)
                            .Send(routed);
                }
                return next(composer.SkipFilters(false), matches);
            }

            private static string GetScheme(Routed routed)
            {
                return Uri.TryCreate(routed.Route, UriKind.Absolute, out var uri)
                     ? uri.Scheme : null;
            }
        }
    }
}
