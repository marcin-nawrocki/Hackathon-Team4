using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Supercontrol.Web.Dashboard.Models;

namespace Supercontrol.Web.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private const string TopLocationsSql =
            @"SELECT customercountry, COUNT(1) AS c
              FROM bookings
              LEFT JOIN customers ON bookings.customerID = customers.customerID
              WHERE bookingdate >= '2026-07-01'
              GROUP BY customercountry
              ORDER BY c DESC";

        public ActionResult Index()
        {
            using (var db = new Supercontrol2Context())
            {
                ViewBag.TopLocations = db.Database
                    .SqlQuery<TopLocationDto>(TopLocationsSql)
                    .ToList();
            }

            return View();
        }
    }
}
