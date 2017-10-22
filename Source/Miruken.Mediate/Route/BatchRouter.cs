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
        private class Request
        {
            public object                           Message;
            public Promise                          Promise;
            public Promise<object>.ResolveCallbackT Resolve;
            public RejectCallback                   Reject;
        }

        private readonly Dictionary<string, List<Request>> _groups;

        protected BatchRouter()
        {
            _groups = new Dictionary<string, List<Request>>();
        }

        [Mediates]
        public Task<object> Route(Routed routed, Command command)
        {
            List<Request> group;
            var route = routed.Route;
            if (!_groups.TryGetValue(route, out group))
            {
                group = new List<Request>();
                _groups.Add(route, group);
            }
            var message = command.Many
                        ? new Publish(routed.Message)
                        : routed.Message;            
            var request = new Request { Message = message };
            var promise = new Promise<object>(
                ChildCancelMode.Any, (resolve, reject) =>
            {
                request.Resolve = resolve;
                request.Reject  = reject;
            });
            request.Promise = promise;
            group.Add(request);
            return promise;
        }

        public object Complete(IHandler composer)
        {
            return Promise.All(_groups.Select(group =>
            {
                var uri      = group.Key;
                var requests = group.Value;
                if (requests.Count == 1)
                {
                    var request = requests[0];
                    return composer.Send(request.Message.RouteTo(uri))
                        .Then((resp, s) =>
                        {
                            request.Resolve(resp, s);
                            return Tuple.Create(uri, new[] {resp});
                        })
                        .Catch((ex, s) =>
                        {
                            request.Reject(ex, s);
                            return Tuple.Create(uri, new object[] {ex});
                        });
                }
                var messages = requests.Select(r => r.Message).ToArray();
                return composer.Send(CreateBatch(messages)
                    .RouteTo(uri)).Then((result, s) =>
                {
                    var responses = result.Responses;
                    for (var i = responses.Length; i < requests.Count; ++i)
                        requests[i].Promise.Cancel();
                    return Tuple.Create(uri, responses
                        .Select((response, i) => response.Match(
                            failure =>
                            {
                                requests[i].Reject(failure, s);
                                return failure;
                            },
                            success =>
                            {
                                requests[i].Resolve(success, s);
                                return success;
                            })).ToArray());
                });
            }).ToArray());
        }

        protected virtual Scheduled CreateBatch(object[] requests)
        {
            return new Concurrent { Requests = requests };
        }
    }
}
