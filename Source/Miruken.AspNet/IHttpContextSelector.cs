namespace Miruken.AspNet
{
    using System.Net.Http;
    using System.Web;
    using Callback;
    using Context;

    public interface IHttpContextSelector : IResolving
    {
        IContext SelectApiContext(HttpRequestMessage request);
        IContext SelectMvcContext(HttpApplication request);
    }
}
