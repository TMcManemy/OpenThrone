using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace OpenThrone.Web
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MvcHandler.DisableMvcResponseHeader = true;
            StallCache.Initialize();
        }

        protected void Application_PreSendRequestHeaders(Object source, EventArgs e)
        {
            var app = source as HttpApplication;
            if (app == null) return;

            var headers = app.Context.Response.Headers;
            headers.Remove("Server");
        }
    }
}