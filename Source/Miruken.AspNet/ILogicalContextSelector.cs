namespace Miruken.AspNet
{
    using System.Net.Http;
    using System.Web;
    using Callback;
    using Context;

    public interface ILogicalContextSelector : IResolving
    {
        Context SelectApiContext(HttpRequestMessage request);
        Context SelectMvcContext(HttpContextBase request);
    }
}
