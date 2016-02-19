using System.Web.Mvc;

namespace OpenThrone.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var stalls = StallCache.AllStalls();
            return View(stalls);
        }
    }
}