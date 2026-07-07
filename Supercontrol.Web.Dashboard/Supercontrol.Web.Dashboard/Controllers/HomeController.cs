using System;
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
              WHERE bookingdate >= @p0
              GROUP BY customercountry
              ORDER BY c DESC";

        private const string AffiliatesSql =
            @"SELECT affiliateId, COUNT(1) AS c
              FROM bookings
              WHERE bookingdate >= @p0
              GROUP BY affiliateId
              ORDER BY c DESC";

        private const string BookingsPerHourSql =
            @"SELECT COUNT(1) AS c, HOUR(bookingdate) AS h
              FROM bookings
              WHERE bookingdate >= @p0
              GROUP BY HOUR(bookingdate)
              ORDER BY c DESC";

        private const string TotalBookingsSql =
            @"SELECT COUNT(1) AS h
              FROM bookings
              WHERE bookingdate >= @p0";

        public ActionResult Index(DateTime? fromDate)
        {
            var filterDate = fromDate?.Date ?? new DateTime(2026, 7, 1);

            ViewBag.FromDate = filterDate.ToString("yyyy-MM-dd");
            ViewBag.FromDateDisplay = filterDate.ToString("d MMMM yyyy");

            using (var db = new Supercontrol2Context())
            {
                ViewBag.TotalBookings = db.Database
                    .SqlQuery<TotalBookingsDto>(TotalBookingsSql, filterDate)
                    .FirstOrDefault()?.H ?? 0;

                ViewBag.TopLocations = db.Database
                    .SqlQuery<TopLocationDto>(TopLocationsSql, filterDate)
                    .ToList();

                ViewBag.Affiliates = db.Database
                    .SqlQuery<AffiliateDto>(AffiliatesSql, filterDate)
                    .ToList();

                var hourlyRaw = db.Database
                    .SqlQuery<BookingsPerHourDto>(BookingsPerHourSql, filterDate)
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
