using System.Collections.Generic;

namespace Supercontrol.Web.Dashboard.Models
{
    public class DashboardData
    {
        public decimal DailyIncome { get; set; }
        public int TotalBookings { get; set; }
        public List<TopLocationDto> TopLocations { get; set; } = new List<TopLocationDto>();
        public List<AffiliateDto> Affiliates { get; set; } = new List<AffiliateDto>();
        public List<BookingsPerHourDto> BookingsPerHour { get; set; } = new List<BookingsPerHourDto>();
    }
}
