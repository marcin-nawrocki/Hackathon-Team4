using System;

namespace Supercontrol.Web.Dashboard.Models
{
    // Raw-SQL result shapes. These are intentionally plain POCOs (not EF entities /
    // DbSets) - Database.SqlQuery<T> maps result columns onto these by name, so we
    // never ask EF to model the underlying tables (which have 90-400+ legacy columns
    // each). See DashboardService for the queries that populate them.

    public class KpiValue
    {
        public long Value { get; set; }
    }

    public class RecentBookingRow
    {
        public long BookingID { get; set; }
        public DateTime? BookingDate { get; set; }
        public string BookingType { get; set; }
        public string SourceID { get; set; }
        public string OwnerCompany { get; set; }
        public string CurrencyCode { get; set; }
        public string CustomerTown { get; set; }
        public string CustomerCountry { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string BookingStatus { get; set; }
        public decimal? FullRate { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public string CottageName { get; set; }
        public string CottageTown { get; set; }
    }

    public class CountryOriginRow
    {
        public string CountryCode { get; set; }
        public int BookingCount { get; set; }
    }

    public class ChannelRow
    {
        public string Channel { get; set; }
        public int BookingCount { get; set; }
    }

    public class DailyTrendRow
    {
        public DateTime Day { get; set; }
        public int BookingCount { get; set; }
    }
}
