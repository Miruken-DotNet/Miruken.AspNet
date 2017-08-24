namespace Miruken.Mediate.Workflow
{
    using System;
    using System.Collections.Generic;
    using Callback;
    using Callback.Policy;

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class SendReturnMessage : Attribute, 
        IFilterProvider, IFilter<object, object>
    {
        public int? Order { get; set; }

        IEnumerable<IFilter> IFilterProvider.GetFilters(MethodBinding binding,
            Type callbackType, Type logicalResultType, IHandler composer)
        {
            yield return this;
        }

        public object Next(object callback, MethodBinding method, 
            IHandler composer, NextDelegate<object> next)
        {
            var result = next();
            return result;
        }
    }
}
