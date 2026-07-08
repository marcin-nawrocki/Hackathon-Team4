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

        private const string DailyIncomeSql =
            @"SELECT SUM(bookingfullrate) AS Total
              FROM bookings
              LEFT JOIN booking_details ON bookings.bookingId = booking_details.bookingId
              WHERE bookingdate >= @p0";

        public ActionResult Index(DateTime? fromDate)
        {
            var filterDate = fromDate?.Date ?? new DateTime(2026, 7, 1);

            ViewBag.FromDate = filterDate.ToString("yyyy-MM-dd");
            ViewBag.FromDateDisplay = filterDate.ToString("d MMMM yyyy");

            var data = LoadDashboardData(filterDate);

            ViewBag.DailyIncome = data.DailyIncome;
            ViewBag.TotalBookings = data.TotalBookings;
            ViewBag.TopLocations = data.TopLocations;
            ViewBag.Affiliates = data.Affiliates;
            ViewBag.BookingsPerHour = data.BookingsPerHour;

            return View();
        }

        [HttpGet]
        public ActionResult Data(DateTime? fromDate)
        {
            var filterDate = fromDate?.Date ?? new DateTime(2026, 7, 1);
            var data = LoadDashboardData(filterDate);

            var payload = new
            {
                dailyIncomeDisplay = data.DailyIncome.ToString("C0"),
                totalBookingsDisplay = data.TotalBookings.ToString("N0"),
                topLocations = data.TopLocations.Select(l => new
                {
                    name = string.IsNullOrWhiteSpace(l.CustomerCountry) ? "Unknown" : l.CustomerCountry,
                    value = l.C,
                    valueDisplay = l.C.ToString("N0")
                }),
                bookingsPerHour = new
                {
                    labels = data.BookingsPerHour.Select(b => b.H.ToString("00") + ":00"),
                    values = data.BookingsPerHour.Select(b => b.C)
                },
                affiliates = new
                {
                    labels = data.Affiliates.Select(a => string.IsNullOrWhiteSpace(a.AffiliateId) ? "None" : a.AffiliateId),
                    values = data.Affiliates.Select(a => a.C)
                }
            };

            return Json(payload, JsonRequestBehavior.AllowGet);
        }

        private DashboardData LoadDashboardData(DateTime filterDate)
        {
            using (var db = new Supercontrol2Context())
            {
                var hourlyLookup = db.Database
                    .SqlQuery<BookingsPerHourDto>(BookingsPerHourSql, filterDate)
                    .ToDictionary(x => x.H, x => x.C);

                return new DashboardData
                {
                    DailyIncome = db.Database
                        .SqlQuery<DailyIncomeDto>(DailyIncomeSql, filterDate)
                        .FirstOrDefault()?.Total ?? 0m,

                    TotalBookings = db.Database
                        .SqlQuery<TotalBookingsDto>(TotalBookingsSql, filterDate)
                        .FirstOrDefault()?.H ?? 0,

                    TopLocations = db.Database
                        .SqlQuery<TopLocationDto>(TopLocationsSql, filterDate)
                        .ToList(),

                    Affiliates = db.Database
                        .SqlQuery<AffiliateDto>(AffiliatesSql, filterDate)
                        .ToList(),

                    BookingsPerHour = Enumerable.Range(0, 24)
                        .Select(h => new BookingsPerHourDto
                        {
                            H = h,
                            C = hourlyLookup.ContainsKey(h) ? hourlyLookup[h] : 0
                        })
                        .ToList()
                };
            }
        }
    }
}
