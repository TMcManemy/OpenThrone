using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OpenThrone.Web.App_Start.Startup))]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
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