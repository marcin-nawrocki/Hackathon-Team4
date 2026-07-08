using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Supercontrol.Web.Dashboard.Helpers;
using Supercontrol.Web.Dashboard.Models;

namespace Supercontrol.Web.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private const string TopLocationsSql =
            @"SELECT NULLIF(TRIM(customercountry), '') AS customercountry, COUNT(1) AS c
              FROM bookings
              LEFT JOIN customers ON bookings.customerID = customers.customerID
              WHERE bookingdate >= @p0
              GROUP BY NULLIF(TRIM(customercountry), '')
              HAVING customercountry IS NOT NULL
              ORDER BY c DESC
              LIMIT 10";

        private const string AffiliatesSql =
            @"SELECT NULLIF(TRIM(affiliateId), '') AS affiliateId, COUNT(1) AS c
              FROM bookings
              WHERE bookingdate >= @p0
                AND TRIM(affiliateId) <> 'ImportAvailabilityService'
              GROUP BY NULLIF(TRIM(affiliateId), '')
              HAVING affiliateId IS NOT NULL
              ORDER BY c DESC
              LIMIT 5";

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
            var filterDate = fromDate?.Date ?? DateTime.Today;
            var data = LoadDashboardData(filterDate);

            ViewBag.FromDate = filterDate.ToString("yyyy-MM-dd");
            ViewBag.FromDateDisplay = filterDate.ToString("d MMMM yyyy");
            ViewBag.DailyIncome = data.DailyIncome;
            ViewBag.TotalBookings = data.TotalBookings;
            ViewBag.TopLocations = data.TopLocations;
            ViewBag.MapGeoJson = TopLocationsMapBuilder.ToGeoJson(data.TopLocations);
            ViewBag.Affiliates = data.Affiliates;
            ViewBag.BookingsPerHour = data.BookingsPerHour;

            return View();
        }

        public ActionResult DashboardData(DateTime? fromDate)
        {
            var filterDate = fromDate?.Date ?? DateTime.Today;
            var data = LoadDashboardData(filterDate);

            var payload = new
            {
                dailyIncome = data.DailyIncome.ToString("C0"),
                totalBookings = data.TotalBookings.ToString("N0"),
                fromDateDisplay = filterDate.ToString("d MMMM yyyy"),
                topLocations = data.TopLocations.Select((l, i) => new
                {
                    rank = i + 1,
                    country = string.IsNullOrWhiteSpace(l.CustomerCountry) ? "Unknown" : l.CustomerCountry,
                    count = l.C.ToString("N0"),
                    value = l.C,
                    flag = CountryFlag.UrlFor(l.CustomerCountry)
                }),
                affiliates = new
                {
                    labels = data.Affiliates.Select(a => string.IsNullOrWhiteSpace(a.AffiliateId) ? "None" : a.AffiliateId),
                    data = data.Affiliates.Select(a => a.C)
                },
                bookingsPerHour = new
                {
                    labels = data.BookingsPerHour.Select(b => b.H.ToString("00") + ":00"),
                    data = data.BookingsPerHour.Select(b => b.C)
                },
                mapGeoJson = TopLocationsMapBuilder.BuildFeatureCollection(data.TopLocations)
            };

            return Json(payload, JsonRequestBehavior.AllowGet);
        }

        private DashboardSnapshot LoadDashboardData(DateTime filterDate)
        {
            using (var db = new Supercontrol2Context())
            {
                var dailyIncome = db.Database
                    .SqlQuery<DailyIncomeDto>(DailyIncomeSql, filterDate)
                    .FirstOrDefault()?.Total ?? 0m;

                var totalBookings = db.Database
                    .SqlQuery<TotalBookingsDto>(TotalBookingsSql, filterDate)
                    .FirstOrDefault()?.H ?? 0;

                var topLocations = db.Database
                    .SqlQuery<TopLocationDto>(TopLocationsSql, filterDate)
                    .ToList();

                var affiliates = db.Database
                    .SqlQuery<AffiliateDto>(AffiliatesSql, filterDate)
                    .ToList();

                var hourlyLookup = db.Database
                    .SqlQuery<BookingsPerHourDto>(BookingsPerHourSql, filterDate)
                    .ToList()
                    .ToDictionary(x => x.H, x => x.C);

                var bookingsPerHour = Enumerable.Range(0, 24)
                    .Select(h => new BookingsPerHourDto
                    {
                        H = h,
                        C = hourlyLookup.ContainsKey(h) ? hourlyLookup[h] : 0
                    })
                    .ToList();

                return new DashboardSnapshot
                {
                    DailyIncome = dailyIncome,
                    TotalBookings = totalBookings,
                    TopLocations = topLocations,
                    Affiliates = affiliates,
                    BookingsPerHour = bookingsPerHour
                };
            }
        }

        private class DashboardSnapshot
        {
            public decimal DailyIncome { get; set; }
            public int TotalBookings { get; set; }
            public List<TopLocationDto> TopLocations { get; set; }
            public List<AffiliateDto> Affiliates { get; set; }
            public List<BookingsPerHourDto> BookingsPerHour { get; set; }
        }
    }
}
