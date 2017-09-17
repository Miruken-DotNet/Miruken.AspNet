namespace Miruken.Mediate.Route
{
    using Callback;
    using Concurrency;

    public class PassThroughRouter : PipelineHandler
    {
        public const string Scheme = "pass-through";

        [Mediates]
        public Promise Route(Routed request, IHandler composer)
        {
            return request.Route == Scheme
                 ? composer.Send(request.Message)
                 : null;
        }
    }
}
