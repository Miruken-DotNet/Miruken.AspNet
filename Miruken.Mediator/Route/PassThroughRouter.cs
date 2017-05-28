namespace Miruken.Mediator.Route
{
    using Callback;
    using Concurrency;

    public class PassThroughRouter : IRouter
    {
        public const string Scheme = "pass-through";

        public bool CanRoute(Routed route)
        {
            return route.Route == Scheme;
        }

        public Promise Route(Routed routed, IHandler composer)
        {
            return composer.Send(routed.Message);
        }
    }
}
