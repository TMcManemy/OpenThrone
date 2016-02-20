using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OpenThrone.Web.App_Start.Startup))]
namespace OpenThrone.Web.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}