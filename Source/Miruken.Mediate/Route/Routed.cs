namespace Miruken.Mediate.Route
{
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
    }
}
