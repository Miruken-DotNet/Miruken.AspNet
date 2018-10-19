
namespace Miruken.AspNet.TestWeb.Controllers
{
    using System;
    using System.Web.Mvc;
    using Castle.Tests;
    using Http;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            TestMirukenHttpFromMVC();

            return View();
        }

        public void TestMirukenHttpFromMVC()
        {
            var uri = $"{Request.Url.Scheme}://{Request.Url.Authority}/process";

            var message = new CreatePlayer
            {
                Player = new Player
                {
                    Name = "a",
                    Person = new Person
                    {
                        DOB = DateTime.Now
                    }
                }
            };

            HttpApiClient.Send(message, uri)
                .GetAwaiter().GetResult();
        }
    }
}
