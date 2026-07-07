using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Supercontrol.Web.Dashboard.Models;

namespace Supercontrol.Web.Dashboard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // "Supercontrol2Reader" is a read-only connection; never let EF try to
            // create/migrate the database schema against it.
            Database.SetInitializer<Supercontrol2Context>(null);
        }
    }
}
