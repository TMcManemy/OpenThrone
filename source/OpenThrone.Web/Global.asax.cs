using System.Web.Http;

namespace OpenThrone.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            StallCache.Initialize();
        }
    }
}
