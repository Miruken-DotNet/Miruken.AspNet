namespace Miruken.Mediate.Schedule
{
    public class Publish : MessageDecorator
    {
        public Publish()
        {
        }

        public Publish(object request)
            : base(request)
        {
        }
    }
}
