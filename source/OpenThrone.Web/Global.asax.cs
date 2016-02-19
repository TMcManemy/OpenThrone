using System.Web.Http;
using System.Web.Routing;

namespace OpenThrone.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            StallCache.Initialize();
        }
    }
}