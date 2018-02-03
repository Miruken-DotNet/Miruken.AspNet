namespace Miruken.Mediate
{
    using System;

    public interface IRequest<out TResponse> { }

    public class MessageDecorator : IDecorator
    {
        public MessageDecorator()
        {
        }

        public MessageDecorator(object message)
        {
            Message = message
                   ?? throw new ArgumentNullException(nameof(message));
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
            get => (IRequest<TResponse>)Message;
            set => Message = value;
        }
    }
}
