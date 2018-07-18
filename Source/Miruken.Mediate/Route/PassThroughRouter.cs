namespace Miruken.Mediate.Route
{
    using Callback;
    using Concurrency;
    using Mediate;

    public class PassThroughRouter : Handler
    {
        public const string Scheme = "pass-through";

        [Handles]
        public Promise Route(Routed request, IHandler composer)
        {
            return request.Route == Scheme
                 ? composer.Send(request.Message)
                 : null;
        }
    }
}
