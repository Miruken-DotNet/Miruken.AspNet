namespace Miruken.Mediate
{
    using System;
    using Api;

    public class MessageDecorator : IDecorator
    {
        public MessageDecorator()
        {
        }

        public MessageDecorator(object message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            Message = message;
        }

        public object Message { get; set; }

        object IDecorator.Decoratee => Message;
    }

    public class RequestDecorator<TResponse> : MessageDecorator, IRequest<TResponse>
    {
        public RequestDecorator()
        {
        }

        public RequestDecorator(IRequest<TResponse> request)
            : base(request)
        {
        }

        public IRequest<TResponse> Request
        {
            get { return (IRequest<TResponse>)Message; }
            set { Message = value; }
        }
    }
}
