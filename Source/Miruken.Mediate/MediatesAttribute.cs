namespace Miruken.Mediate
{
    using Api;
    using Callback;
    using Callback.Policy;

    public class MediatesAttribute : CategoryAttribute
    {
        public MediatesAttribute()
        {
        }

        public MediatesAttribute(object key)
        {
            InKey = key;
        }

        public override CallbackPolicy CallbackPolicy => Policy;

        public static readonly CallbackPolicy Policy =
            ContravariantPolicy.Create<Command>(r => r.Callback,
                x => x.MatchCallbackMethod(Return.Alias("resp"),
                           x.Target.OfType(typeof(IRequest<>), "resp"))
                      .MatchCallbackMethod(x.Target)
                      .MatchMethod(x.Callback)
            );
    }
}
