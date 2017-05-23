namespace Miruken.Mediator.Route
{
    using Callback;
    using Concurrency;

    public interface IRouter
    {
        bool CanRoute(Routed route);

        Promise Route(Routed route, IHandler composer);
    }
}
