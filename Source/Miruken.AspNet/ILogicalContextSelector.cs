namespace Miruken.AspNet
{
    using System.Net.Http;
    using System.Web;
    using Callback;
    using Context;

    public interface ILogicalContextSelector : IResolving
    {
        IContext SelectApiContext(HttpRequestMessage request);
        IContext SelectMvcContext(HttpContextBase request);
    }
}
