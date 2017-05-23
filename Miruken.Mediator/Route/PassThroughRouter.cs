namespace Miruken.Mediator.Route
{
    using Callback;
    using Concurrency;

    public class PassThroughRouter : IRouter
    {
        public bool CanRoute(Routed route)
        {
            return route.Route == "pass-through";
        }

        public Promise Route(Routed route, IHandler composer)
        {
            var request = ((IDecorator)route).Decoratee;
            return composer.Send(request);
        }
    }
}
