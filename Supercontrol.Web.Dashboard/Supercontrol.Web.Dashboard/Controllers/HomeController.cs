using System;
using System.Linq;
using System.Web.Mvc;
using Supercontrol.Web.Dashboard.Models;
using Supercontrol.Web.Dashboard.Services;

namespace Supercontrol.Web.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        // 7-day windows keep every query below bounded and index-backed (see DashboardService).
        private const int TrendDays = 14;
        private const int OriginWindowDays = 7;

        private static int GetOrZero(System.Collections.Generic.Dictionary<string, int> dict, string key)
        {
            int value;
            return dict.TryGetValue(key, out value) ? value : 0;
        }

        public ActionResult Index()
        {
            using (var db = new Supercontrol2Context())
            {
                var svc = new DashboardService(db);
                var model = BuildViewModel(svc);
                return View(model);
            }
        }

        /// <summary>Polled by the dashboard every 30s to refresh KPIs/feed/chart/donut without a full reload.</summary>
        public ActionResult LiveSummary()
        {
            using (var db = new Supercontrol2Context())
            {
                var svc = new DashboardService(db);
                var model = BuildViewModel(svc);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GeoJSON FeatureCollection of where bookings' customers actually live (customercountry),
        /// not where the properties booked are located. Consumed by dashboard-map.js the same way
        /// the old static Content/data/activity-regions.json was.
        /// </summary>
        public ActionResult BookingOrigins()
        {
            using (var db = new Supercontrol2Context())
            {
                var svc = new DashboardService(db);
                var geo = BuildOriginsGeoJson(svc);
                return Json(geo, JsonRequestBehavior.AllowGet);
            }
        }

        private DashboardViewModel BuildViewModel(DashboardService svc)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var yesterday = today.AddDays(-1);
            var weekAgo = today.AddDays(-(OriginWindowDays - 1));
            var prevWeekAgo = weekAgo.AddDays(-OriginWindowDays);
            var trendStart = today.AddDays(-(TrendDays - 1));

            var bookingsToday = svc.CountBookingsInRange(today, tomorrow);
            var bookingsYesterday = svc.CountBookingsInRange(yesterday, today);

            var origins = svc.GetBookingOriginsByCountry(weekAgo, tomorrow);
            var prevOrigins = svc.GetBookingOriginsByCountry(prevWeekAgo, weekAgo)
                .ToDictionary(o => o.CountryCode, o => o.BookingCount);

            var topCountries = origins
                .Take(8)
                .Select(o =>
                {
                    var geoInfo = CountryGeo.Lookup(o.CountryCode);
                    var prevCount = GetOrZero(prevOrigins, o.CountryCode);
                    return new TopCountryItem
                    {
                        Code = o.CountryCode,
                        Name = geoInfo != null ? geoInfo.Name : o.CountryCode,
                        Flag = CountryGeo.FlagEmoji(o.CountryCode),
                        Value = o.BookingCount,
                        ChangePercent = DashboardViewModel.PercentChange(o.BookingCount, prevCount)
                    };
                })
                .ToList();

            return new DashboardViewModel
            {
                BookingsToday = bookingsToday,
                BookingsTodayChangePercent = DashboardViewModel.PercentChange(bookingsToday, bookingsYesterday),
                EnquiriesToday = svc.CountNewEnquiriesInRange(today, tomorrow),
                BookingsThisWeek = svc.CountBookingsInRange(weekAgo, tomorrow),
                LiveLastHour = svc.CountLiveInLastMinutes(60),
                RecentBookings = svc.GetRecentBookings(25),
                TopCountries = topCountries,
                Channels = svc.GetChannelBreakdown(weekAgo, tomorrow),
                Trend = svc.GetDailyTrend(trendStart, tomorrow)
            };
        }

        private object BuildOriginsGeoJson(DashboardService svc)
        {
            var today = DateTime.Today;
            var weekAgo = today.AddDays(-(OriginWindowDays - 1));
            var prevWeekAgo = weekAgo.AddDays(-OriginWindowDays);

            var origins = svc.GetBookingOriginsByCountry(weekAgo, today.AddDays(1));
            var prevOrigins = svc.GetBookingOriginsByCountry(prevWeekAgo, weekAgo)
                .ToDictionary(o => o.CountryCode, o => o.BookingCount);

            var withGeo = origins
                .Select(o => new { Origin = o, Geo = CountryGeo.Lookup(o.CountryCode) })
                .Where(x => x.Geo != null)
                .ToList();

            var avgCount = withGeo.Count > 0 ? withGeo.Average(x => x.Origin.BookingCount) : 0;

            var features = withGeo.Select(x =>
            {
                var prevCount = GetOrZero(prevOrigins, x.Origin.CountryCode);
                var changePct = DashboardViewModel.PercentChange(x.Origin.BookingCount, prevCount);
                var change = changePct.HasValue ? changePct.Value : 0;
                return new
                {
                    type = "Feature",
                    properties = new
                    {
                        name = x.Geo.Name,
                        country = x.Origin.CountryCode,
                        value = x.Origin.BookingCount,
                        score = x.Origin.BookingCount >= avgCount ? "high" : "low",
                        change = change
                    },
                    geometry = new
                    {
                        type = "Point",
                        coordinates = new[] { x.Geo.Lng, x.Geo.Lat }
                    }
                };
            }).ToList();

            return new
            {
                type = "FeatureCollection",
                features = features
            };
        }
    }
}
