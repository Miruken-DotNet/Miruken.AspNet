namespace Miruken.Mediate.Schedule
{
    public class Publish : MessageDecorator
    {
        public Publish()
        {
        }

        public Publish(object message)
            : base(message)
        {
        }
    }

    public static class PublishExtensions
    {
        public static Publish Publish(this object message)
        {
            return new Publish(message);
        }
    }
}
