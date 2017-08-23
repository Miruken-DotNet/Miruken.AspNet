namespace Miruken.Mediator
{
    using Callback;
    using Callback.Policy;

    public class MediatesAttribute : DefinitionAttribute
    {
        public MediatesAttribute()
        {          
        }

        public MediatesAttribute(object key)
        {
            Key = key;
        }

        public override CallbackPolicy CallbackPolicy => Policy;

        public static readonly CallbackPolicy Policy =
            ContravariantPolicy.Create<Command>(r => r.Callback,
                x => x.MatchCallbackMethod(Return.Is("resp"),
                           x.Target.OfType(typeof(IRequest<>), "resp"))
                      .MatchCallbackMethod(x.Target)
                      .MatchMethod(x.Callback)
            );
    }
}
