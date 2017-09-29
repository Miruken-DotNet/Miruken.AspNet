namespace Miruken.AspNet
{
    using System.Net.Http;
    using Callback;
    using Context;

    public interface IClientContextSelector : IResolving
    {
        IContext SelectContext(HttpRequestMessage request);
    }
}
