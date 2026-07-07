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

        private const string AffiliatesSql =
            @"SELECT affiliateId, COUNT(1) AS c
              FROM bookings
              WHERE bookingdate >= '2026-07-01'
              GROUP BY affiliateId
              ORDER BY c DESC";

        private const string BookingsPerHourSql =
            @"SELECT COUNT(1) AS c, HOUR(bookingdate) AS h
              FROM bookings
              WHERE bookingdate >= '2026-07-01'
              GROUP BY HOUR(bookingdate)
              ORDER BY c DESC";

        public ActionResult Index()
        {
            using (var db = new Supercontrol2Context())
            {
                ViewBag.TopLocations = db.Database
                    .SqlQuery<TopLocationDto>(TopLocationsSql)
                    .ToList();

                ViewBag.Affiliates = db.Database
                    .SqlQuery<AffiliateDto>(AffiliatesSql)
                    .ToList();

                var hourlyRaw = db.Database
                    .SqlQuery<BookingsPerHourDto>(BookingsPerHourSql)
                    .ToList();

                var hourlyLookup = hourlyRaw.ToDictionary(x => x.H, x => x.C);
                ViewBag.BookingsPerHour = Enumerable.Range(0, 24)
                    .Select(h => new BookingsPerHourDto
                    {
                        H = h,
                        C = hourlyLookup.ContainsKey(h) ? hourlyLookup[h] : 0
                    })
                    .ToList();
            }

            return View();
        }
    }
}