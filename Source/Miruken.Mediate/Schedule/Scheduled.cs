namespace Miruken.Mediate.Schedule
{
    using System;

    public abstract class Scheduled : IRequest<ScheduledResult>
    {
        public object[] Requests { get; set; }
    }

    public class ScheduledResult
    {
        public Try<Exception, object>[] Responses { get; set; }
    }
}
