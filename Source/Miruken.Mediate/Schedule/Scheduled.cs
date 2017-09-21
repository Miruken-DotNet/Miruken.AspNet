namespace Miruken.Mediate.Schedule
{
    public abstract class Scheduled : IRequest<ScheduleResult>
    {
        public object[] Requests { get; set; }
    }

    public class ScheduleResult
    {
        public object[] Responses { get; set; }
    }
}
