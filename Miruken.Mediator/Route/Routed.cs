namespace Miruken.Mediator.Route
{
    using System;
    using Callback;

    public abstract class Routed : IDecorator
    {
        public string Route { get; set; }
        public string Tag   { get; set; }

        object IDecorator.Decoratee => Decoratee;

        protected abstract object Decoratee { get; }
    }

    public class RoutedRequest<TResponse> : Routed,
        IRequestDecorator<TResponse>
    {
        public RoutedRequest()
        {
        }

        public RoutedRequest(IRequest<TResponse> request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Request = request;
        }

        public IRequest<TResponse> Request { get; set; }

        protected override object Decoratee => Request;
    }

    public class RoutedRequest : Routed, IRequestDecorator
    {
        public RoutedRequest()
        {
        }

        public RoutedRequest(IRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Request = request;
        }

        public IRequest Request { get; set; }

        protected override object Decoratee => Request;
    }

    public class RoutedNotification : Routed, INotificationDecorator
    {
        public RoutedNotification()
        {
        }

        public RoutedNotification(INotification notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            Notification = notification;
        }

        public INotification Notification { get; set; }

        protected override object Decoratee => Notification;
    }
}
