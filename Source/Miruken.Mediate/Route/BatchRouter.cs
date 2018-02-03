namespace Miruken.Mediate.Route
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Callback;
    using Concurrency;
    using Schedule;

    [Unmanaged]
    public class BatchRouter : PipelineHandler, IBatching
    {
        private readonly Dictionary<string, List<Request>> _groups;

        public BatchRouter()
        {
            _groups = new Dictionary<string, List<Request>>();
        }

        [Mediates]
        public Promise<object> Route(Routed routed, Command command)
        {
            var route = routed.Route;
            if (!_groups.TryGetValue(route, out var group))
            {
                group = new List<Request>();
                _groups.Add(route, group);
            }
            var message = command.Many
                        ? new Publish(routed.Message)
                        : routed.Message;
            var request = new Request(message);
            group.Add(request);
            return request.Promise;
        }

        public object Complete(IHandler composer)
        {
            var complete = Promise.All(_groups.Select(group =>
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
                return composer.Send(new Concurrent {Requests  = messages}
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
            }).Cast<object>().ToArray());
            _groups.Clear();
            return complete;
        }

        private class Request
        {
            public object                           Message { get; }
            public Promise<object>                  Promise { get; }
            public Promise<object>.ResolveCallbackT Resolve { get; private set; }
            public RejectCallback                   Reject  { get; private set; }

            public Request(object message)
            {
                Message = message;
                Promise = new Promise<object>((resolve, reject) =>
                {
                    Resolve = resolve;
                    Reject  = reject;
                });
            }
        }
    }
}
