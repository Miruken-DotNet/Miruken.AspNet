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

        public Promise Route(Routed routed, IHandler composer)
        {
            return composer.Send(routed.Message);
        }
    }
}
