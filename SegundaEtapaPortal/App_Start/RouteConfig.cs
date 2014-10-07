using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ULAPW
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

           // routes.MapRoute(
           //    name: "Logueo",
           //    url: "{controller}/{action}/{user}/{token}/{ruc}",
           //    defaults: new { controller = "Home", action = "Index", ruc = UrlParameter.Optional },
           //    namespaces: new string[] { "UPW.Controllers" }
           //);
        }
    }
}
