using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EqCrm {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "POS",
                url: "POS/{controller}/{action}/{id}",
                defaults: new { controller = "POS", action = "Index", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "POS_SV",
                url: "POS_SV/{controller}/{action}/{id}",
                defaults: new { controller = "POS", action = "Index", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
