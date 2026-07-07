using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Supercontrol.Web.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = new Models.Supercontrol2Context();
            var v = db.Database.SqlQuery<int>("SELECT 1").FirstOrDefault();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}