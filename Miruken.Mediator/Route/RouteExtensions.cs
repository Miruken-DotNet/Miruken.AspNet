namespace Miruken.Mediator.Route
{
    public static class RouteExtensions
    {
        public static RoutedRequest RouteTo(
            this IRequest request, string route, string tag = null)
        {
            return new RoutedRequest(request)
            {
                Route = route,
                Tag   = tag
            };
        }

        public static RoutedRequest<TResponse> RouteTo<TResponse>(
            this IRequest<TResponse> request, string route, string tag = null)
        {
            return new RoutedRequest<TResponse>(request)
            {
                Route = route,
                Tag   = tag
            };
        }

        public static RoutedNotification RouteTo(
            this INotification notification, string route, string tag = null)
        {
            return new RoutedNotification(notification)
            {
                Route = route,
                Tag   = tag
            };
        }
    }
}
