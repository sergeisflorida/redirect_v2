using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace redirect_v2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "manager",
               url: "manager/{action}",
               defaults: new { controller = "Manager", action = "Index", id = UrlParameter.Optional }
           );

            /*
             * routes.MapRoute(
               name: "r",
               url: "r/{id}",
               defaults: new { controller = "r", action = "Index", id = UrlParameter.Optional }
            );
            */

            routes.MapRoute(
                name: "Default",
                url: "{id}",
                defaults: new { controller = "r", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
