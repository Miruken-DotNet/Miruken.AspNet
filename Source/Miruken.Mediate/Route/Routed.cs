namespace Miruken.Mediate.Route
{
    using Api;

    public class Routed : MessageDecorator
    {
        public Routed()
        {           
        }

        public Routed(object message) : base(message)
        {
        }

        public string Route { get; set; }
        public string Tag   { get; set; }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other?.GetType() != GetType())
                return false;

            return other is Routed otherRouted
                   && Equals(Route, otherRouted.Route)
                   && Equals(Tag, otherRouted.Tag);
        }

        public override int GetHashCode()
        {
            return (Route?.GetHashCode() ?? 0) * 31 +
                   (Tag?.GetHashCode() ?? 0);
        }
    }

    public class RoutedRequest<TResponse> : Routed, IRequest<TResponse>
    {
        public RoutedRequest()
        {
        }

        public RoutedRequest(IRequest<TResponse> request)
            : base(request)
        {
        }

        public IRequest<TResponse> Request
        {
            get => (IRequest<TResponse>) Message;
            set => Message = value;
        }

        public override bool Equals(object other)
        {
            if (!base.Equals(other)) return false;
            return other is RoutedRequest<TResponse> otherRequest
                   && Equals(Request, otherRequest.Request);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * 31 + (Request?.GetHashCode() ?? 0);
        }
    }
}
