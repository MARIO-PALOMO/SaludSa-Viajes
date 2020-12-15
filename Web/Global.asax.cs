using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SaludSA
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        //void Session_Start(object sender, EventArgs e)
        //{
        //    Session.Timeout = 43200;
        //}
    }
}