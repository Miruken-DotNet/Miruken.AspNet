namespace Miruken.Mediator.Route
{
    using System;
    using System.Linq;
    using Callback;
    using Concurrency;

    public class RouteHandler : Mediator
    {
        private readonly IRouter[] _routers;

        public RouteHandler(params IRouter[] routers)
        {
            _routers = routers;
        }

        [Mediates]
        public Promise Route(RoutedRequest request, IHandler composer)
        {
            var router = SelectRouter(request);
            return router.Route(request, composer);
        }

        [Mediates]
        public Promise<TResponse> Route<TResponse>(
            RoutedRequest<TResponse> request, IHandler composer)
        {
            var router = SelectRouter(request);
            return router.Route(request, composer).Cast<TResponse>();
        }

        [Mediates]
        public Promise Route(RoutedNotification notification, IHandler composer)
        {
            var router = SelectRouter(notification);
            return router.Route(notification, composer);
        }

        private IRouter SelectRouter(Routed routed)
        {
            var router = _routers.FirstOrDefault(r => r.CanRoute(routed));
            if (router != null) return router;
                throw new NotSupportedException($"Unrecognized route '{routed.Route}'");
        }
    }
}
