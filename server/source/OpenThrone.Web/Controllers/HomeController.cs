using System.Linq;
using System.Web.Mvc;

namespace OpenThrone.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var subdomain = Request.Url.Host.Split('.')[0];
            var stalls = StallCache.AllStalls().Where(s => s.Location == subdomain);

            return stalls.Any() ? View(subdomain, stalls) : View("Index");
        }
    }
}