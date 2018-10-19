
namespace Miruken.AspNet.TestWeb.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Castle.Tests;
    using Http;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var response = TestMirukenHttpFromMVC();

            return View();
        }

        public async Task<PlayerResponse> TestMirukenHttpFromMVC()
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

            return await HttpApiClient.Send(message, uri);
        }
    }
}
