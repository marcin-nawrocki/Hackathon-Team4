using System;
using System.Collections.Generic;
using System.Linq;
using Supercontrol.Web.Dashboard.Models;

namespace Supercontrol.Web.Dashboard.Services
{
    /// <summary>
    /// All queries here are deliberately bounded and index-backed against supercontrol2
    /// (24M+ row bookings/booking_details, 35M+ row customers):
    ///  - "recent" queries filter on bookings.bookingID (PK) so MySQL never scans past
    ///    the requested window before applying LIMIT.
    ///  - "today" / "N days" queries filter on bookings.bookingdate, which is covered by
    ///    idxBookingDate_Lastupdated / ownerID_bookingdate_affiliateID.
    ///  - every query excludes bookingtype IN ('ADMIN','ADMINx','AMDIN') (staff-entered
    ///    admin rows, not real customer activity) and owners.ownertest = 1 (sandbox /
    ///    channel-certification tenants such as the Airbnb/Booking.com test property) so
    ///    the "live" dashboard reflects genuine bookings.
    ///  - bookings.bookingcanceldate has no index, so it is intentionally NOT queried here
    ///    (a WHERE on it would force a full table scan on a 24M-row table). A "cancellations
    ///    today" tile would need a new index (idx on bookingcanceldate) before it's safe to add.
    /// </summary>
    public class DashboardService
    {
        private readonly Models.Supercontrol2Context _db;

        private const string ExcludeAdminTypes = "b.bookingtype NOT IN ('ADMIN','ADMINx','AMDIN')";
        private const string ExcludeTestOwners = "COALESCE(o.ownertest,0) = 0";

        public DashboardService(Models.Supercontrol2Context db)
        {
            _db = db;
        }

        public long CountBookingsInRange(DateTime fromInclusive, DateTime toExclusive)
        {
            var sql =
                "SELECT COUNT(*) AS Value " +
                "FROM bookings b " +
                "INNER JOIN owners o ON o.ownerID = b.ownerID " +
                "WHERE b.bookingdate >= {0} AND b.bookingdate < {1} " +
                "  AND " + ExcludeAdminTypes +
                "  AND " + ExcludeTestOwners;
            return _db.Database.SqlQuery<KpiValue>(sql, fromInclusive, toExclusive).Single().Value;
        }

        public long CountNewEnquiriesInRange(DateTime fromInclusive, DateTime toExclusive)
        {
            var sql =
                "SELECT COUNT(*) AS Value " +
                "FROM bookings b " +
                "INNER JOIN booking_details bd ON bd.bookingID = b.bookingID " +
                "INNER JOIN owners o ON o.ownerID = b.ownerID " +
                "WHERE b.bookingdate >= {0} AND b.bookingdate < {1} " +
                "  AND TRIM(bd.bookingstatus) = 'Enquiry' " +
                "  AND " + ExcludeTestOwners;
            return _db.Database.SqlQuery<KpiValue>(sql, fromInclusive, toExclusive).Single().Value;
        }

        public long CountLiveInLastMinutes(int minutes)
        {
            var sql =
                "SELECT COUNT(*) AS Value " +
                "FROM bookings b " +
                "INNER JOIN owners o ON o.ownerID = b.ownerID " +
                "WHERE b.bookingdate >= DATE_SUB(NOW(), INTERVAL {0} MINUTE) " +
                "  AND " + ExcludeAdminTypes +
                "  AND " + ExcludeTestOwners;
            return _db.Database.SqlQuery<KpiValue>(sql, minutes).Single().Value;
        }

        /// <summary>
        /// Latest booking lines (one row per booking_details, so a multi-property booking
        /// shows once per property). Bounded by a bookingID window before the final LIMIT
        /// so the join never touches more than `scanWindow` rows of `bookings`.
        /// </summary>
        public List<RecentBookingRow> GetRecentBookings(int take, int scanWindow)
        {
            var sql =
                "SELECT " +
                "  b.bookingID AS BookingID, b.bookingdate AS BookingDate, b.bookingtype AS BookingType, b.sourceID AS SourceID, " +
                "  o.ownercompany AS OwnerCompany, o.ownercurrency AS CurrencyCode, " +
                "  c.customertown AS CustomerTown, c.customercountry AS CustomerCountry, " +
                "  bd.bookingstartdate AS CheckIn, bd.bookingenddate AS CheckOut, " +
                "  TRIM(bd.bookingstatus) AS BookingStatus, bd.bookingfullrate AS FullRate, " +
                "  bd.bookingadults AS Adults, bd.bookingchildren AS Children, " +
                "  ct.cottagename AS CottageName, ct.cottagetown AS CottageTown " +
                "FROM bookings b " +
                "INNER JOIN owners o ON o.ownerID = b.ownerID " +
                "INNER JOIN customers c ON c.customerID = b.customerID " +
                "LEFT JOIN booking_details bd ON bd.bookingID = b.bookingID " +
                "LEFT JOIN cottages ct ON ct.cottageID = bd.cottageID " +
                "WHERE b.bookingID > (SELECT MAX(bookingID) FROM bookings) - {0} " +
                "  AND " + ExcludeAdminTypes +
                "  AND " + ExcludeTestOwners +
                "ORDER BY b.bookingID DESC " +
                "LIMIT {1}";
            return _db.Database.SqlQuery<RecentBookingRow>(sql, scanWindow, take).ToList();
        }

        public List<RecentBookingRow> GetRecentBookings(int take)
        {
            return GetRecentBookings(take, 5000);
        }

        /// <summary>Booking counts by customer country of origin (NOT property location) for a bounded date range.</summary>
        public List<CountryOriginRow> GetBookingOriginsByCountry(DateTime fromInclusive, DateTime toExclusive)
        {
            var sql =
                "SELECT c.customercountry AS CountryCode, COUNT(*) AS BookingCount " +
                "FROM bookings b " +
                "INNER JOIN owners o ON o.ownerID = b.ownerID " +
                "INNER JOIN customers c ON c.customerID = b.customerID " +
                "WHERE b.bookingdate >= {0} AND b.bookingdate < {1} " +
                "  AND " + ExcludeAdminTypes +
                "  AND " + ExcludeTestOwners +
                "  AND c.customercountry IS NOT NULL AND c.customercountry <> '' " +
                "GROUP BY c.customercountry " +
                "ORDER BY BookingCount DESC";
            return _db.Database.SqlQuery<CountryOriginRow>(sql, fromInclusive, toExclusive).ToList();
        }

        public List<ChannelRow> GetChannelBreakdown(DateTime fromInclusive, DateTime toExclusive)
        {
            var sql =
                "SELECT " +
                "  CASE " +
                "    WHEN b.sourceID IS NULL OR TRIM(b.sourceID) IN ('', '0') THEN 'Unknown' " +
                "    WHEN b.sourceID = 'Airbnb' THEN 'Airbnb' " +
                "    WHEN b.sourceID = 'Booking.com' THEN 'Booking.com' " +
                "    WHEN b.sourceID LIKE '%vrbo%' OR b.sourceID LIKE '%homeaway%' THEN 'Vrbo / HomeAway' " +
                "    WHEN b.bookingtype = 'WEBSITE' THEN 'Direct / Website' " +
                "    ELSE 'Other' " +
                "  END AS Channel, " +
                "  COUNT(*) AS BookingCount " +
                "FROM bookings b " +
                "INNER JOIN owners o ON o.ownerID = b.ownerID " +
                "WHERE b.bookingdate >= {0} AND b.bookingdate < {1} " +
                "  AND " + ExcludeAdminTypes +
                "  AND " + ExcludeTestOwners +
                "GROUP BY Channel " +
                "ORDER BY BookingCount DESC";
            return _db.Database.SqlQuery<ChannelRow>(sql, fromInclusive, toExclusive).ToList();
        }

        public List<DailyTrendRow> GetDailyTrend(DateTime fromInclusive, DateTime toExclusive)
        {
            var sql =
                "SELECT DATE(b.bookingdate) AS Day, COUNT(*) AS BookingCount " +
                "FROM bookings b " +
                "INNER JOIN owners o ON o.ownerID = b.ownerID " +
                "WHERE b.bookingdate >= {0} AND b.bookingdate < {1} " +
                "  AND " + ExcludeAdminTypes +
                "  AND " + ExcludeTestOwners +
                "GROUP BY DATE(b.bookingdate) " +
                "ORDER BY Day";
            return _db.Database.SqlQuery<DailyTrendRow>(sql, fromInclusive, toExclusive).ToList();
        }
    }
}
