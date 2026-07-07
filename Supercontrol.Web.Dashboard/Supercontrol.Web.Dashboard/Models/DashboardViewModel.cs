using System;
using System.Collections.Generic;

namespace Supercontrol.Web.Dashboard.Models
{
    public class TopCountryItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Flag { get; set; }
        public int Value { get; set; }
        public double? ChangePercent { get; set; }
    }

    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            RecentBookings = new List<RecentBookingRow>();
            TopCountries = new List<TopCountryItem>();
            Channels = new List<ChannelRow>();
            Trend = new List<DailyTrendRow>();
        }

        public long BookingsToday { get; set; }
        public double? BookingsTodayChangePercent { get; set; }
        public long EnquiriesToday { get; set; }
        public long BookingsThisWeek { get; set; }
        public long LiveLastHour { get; set; }

        public List<RecentBookingRow> RecentBookings { get; set; }
        public List<TopCountryItem> TopCountries { get; set; }
        public List<ChannelRow> Channels { get; set; }
        public List<DailyTrendRow> Trend { get; set; }

        public static double? PercentChange(long current, long previous)
        {
            if (previous <= 0) return null;
            return Math.Round((current - previous) / (double)previous * 100.0, 1);
        }
    }
}
