using System.Web;
using Miruken.AspNet;

[assembly: PreApplicationStartMethod(typeof(ContextualModule), "Start")]

namespace Miruken.AspNet
{
    using System.Web;

    public class ContextualModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
        }

        public void Dispose()
        {
        }

        public static void Start()
        {
        }
    }
}
