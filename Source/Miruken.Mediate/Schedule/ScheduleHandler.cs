namespace Miruken.Mediate.Schedule
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;

    public class ScheduleHandler : PipelineHandler
    {
        [Mediates]
        public async Task<ScheduleResult> Concurrent(
            Concurrent concurrent, IHandler composer)
        {
            var requests = concurrent.Requests;
            if (requests == null || requests.Length == 0)
                return new ScheduleResult
                {
                    Responses = Array.Empty<object>()
                };
            var all = Task.WhenAll(requests.Select(req => Process(req, composer)));
            try
            {
                return new ScheduleResult
                {
                    Responses = await all
                };
            }
            catch when (all.Exception != null)
            {
                throw all.Exception;
            }
        }

        [Mediates]
        public async Task<ScheduleResult> Sequential(
            Sequential sequential, IHandler composer)
        {
            var responses = new List<object>();
            var requests  = sequential.Requests;
            if (requests != null && requests.Length > 0)
            {
                foreach (var req in sequential.Requests)
                    responses.Add(await Process(req, composer));
            }
            return new ScheduleResult
            {
                Responses = responses.ToArray()
            };
        }

        [Mediates]
        public ScheduleResult Parallel(Parallel parallel, IHandler composer)
        {
            var requests = parallel.Requests;
            if (requests == null || requests.Length == 0)
                return new ScheduleResult
                {
                    Responses = Array.Empty<object>()
                };
            try
            {
                return new ScheduleResult
                {
                    Responses = requests.AsParallel().Select(
                        req => Process(req, composer).Result).ToArray()
                };
            }
            catch (AggregateException e) when (e.InnerExceptions.Count == 1)
            {
                throw e.InnerExceptions[0];
            }
        }

        public Task Publish(Publish publish, IHandler composer)
        {
            return composer.Publish(publish.Message);
        }

        private static Task<object> Process(object request, IHandler composer)
        {
            return composer.Send(request);
        }
    }
}
