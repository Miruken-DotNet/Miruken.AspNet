namespace Miruken.Mediate.Route
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;
    using Concurrency;
    using Schedule;

    public abstract class BatchRouter : PipelineHandler, IBatching
    {
        private readonly Dictionary<string,
            Tuple<List<object>,
                  List<Promise<object>.ResolveCallbackT>,
                  List<Promise<object>>
            >> _groups;

        protected BatchRouter()
        {
            _groups = new Dictionary<string,
                Tuple<List<object>,
                      List<Promise<object>.ResolveCallbackT>,
                      List<Promise<object>>>>();
        }

        [Mediates]
        public Task<object> Route(Routed routed, Command command)
        {
            Tuple<List<object>,
            List<Promise<object>.ResolveCallbackT>,
            List<Promise<object>>> group;
            var route = routed.Route;
            if (!_groups.TryGetValue(route, out group))
            {
                group = Tuple.Create(
                    new List<object>(),
                    new List<Promise<object>.ResolveCallbackT>(),
                    new List<Promise<object>>());
                _groups.Add(route, group);
            }
            var message = routed.Message;
            if (command.Many) message = new Publish(message);
            group.Item1.Add(message);
            var promise = new Promise<object>(
                (resolve, reject) => group.Item2.Add(resolve));
            group.Item3.Add(promise);
            return promise;
        }

        public object Complete(IHandler composer)
        {
            return Promise.All(_groups.Select(group =>
            {
                var uri      = group.Key;
                var requests = group.Value.Item1;
                var resolves = group.Value.Item2;
                return requests.Count == 1
                     ? composer.Send(requests[0]).Then((resp, s) =>
                     {
                         resolves[0](resp, s);
                         return Tuple.Create(uri, new[] { resp });
                     })
                     : composer.Send(new Concurrent
                     {
                         Requests = group.Value.Item1.ToArray()
                     }.RouteTo(uri)).Then((result, s) =>
                     {
                         var responses = result.Responses;
                         for (var i = 0; i < responses.Length; ++i)
                             resolves[i](responses[i], s);
                         return Tuple.Create(uri, responses);
                     });
            }).ToArray());
        }
    }
}
