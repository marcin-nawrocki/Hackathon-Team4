using System.Collections.Generic;

namespace Supercontrol.Web.Dashboard.Models
{
    public class CountryGeoInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    /// <summary>
    /// Static ISO-3166 alpha-2 -> centroid/name lookup used to plot customers.customercountry
    /// on the map. The database has no lat/long for customers (checked: 0% filled), so
    /// aggregated country counts from SQL are joined against this table in-memory.
    /// </summary>
    public static class CountryGeo
    {
        public static readonly Dictionary<string, CountryGeoInfo> ByCode = new Dictionary<string, CountryGeoInfo>(System.StringComparer.OrdinalIgnoreCase)
        {
            { "AD", new CountryGeoInfo { Code = "AD", Name = "Andorra", Lat = 42.55, Lng = 1.58 } },
            { "AE", new CountryGeoInfo { Code = "AE", Name = "United Arab Emirates", Lat = 23.42, Lng = 53.85 } },
            { "AF", new CountryGeoInfo { Code = "AF", Name = "Afghanistan", Lat = 33.94, Lng = 67.71 } },
            { "AG", new CountryGeoInfo { Code = "AG", Name = "Antigua and Barbuda", Lat = 17.06, Lng = -61.80 } },
            { "AL", new CountryGeoInfo { Code = "AL", Name = "Albania", Lat = 41.15, Lng = 20.17 } },
            { "AM", new CountryGeoInfo { Code = "AM", Name = "Armenia", Lat = 40.07, Lng = 45.04 } },
            { "AO", new CountryGeoInfo { Code = "AO", Name = "Angola", Lat = -11.20, Lng = 17.87 } },
            { "AR", new CountryGeoInfo { Code = "AR", Name = "Argentina", Lat = -38.42, Lng = -63.62 } },
            { "AT", new CountryGeoInfo { Code = "AT", Name = "Austria", Lat = 47.52, Lng = 14.55 } },
            { "AU", new CountryGeoInfo { Code = "AU", Name = "Australia", Lat = -25.27, Lng = 133.78 } },
            { "AZ", new CountryGeoInfo { Code = "AZ", Name = "Azerbaijan", Lat = 40.14, Lng = 47.58 } },
            { "BA", new CountryGeoInfo { Code = "BA", Name = "Bosnia and Herzegovina", Lat = 43.92, Lng = 17.68 } },
            { "BB", new CountryGeoInfo { Code = "BB", Name = "Barbados", Lat = 13.19, Lng = -59.54 } },
            { "BD", new CountryGeoInfo { Code = "BD", Name = "Bangladesh", Lat = 23.68, Lng = 90.36 } },
            { "BE", new CountryGeoInfo { Code = "BE", Name = "Belgium", Lat = 50.50, Lng = 4.47 } },
            { "BG", new CountryGeoInfo { Code = "BG", Name = "Bulgaria", Lat = 42.73, Lng = 25.49 } },
            { "BH", new CountryGeoInfo { Code = "BH", Name = "Bahrain", Lat = 26.07, Lng = 50.56 } },
            { "BM", new CountryGeoInfo { Code = "BM", Name = "Bermuda", Lat = 32.32, Lng = -64.75 } },
            { "BO", new CountryGeoInfo { Code = "BO", Name = "Bolivia", Lat = -16.29, Lng = -63.59 } },
            { "BR", new CountryGeoInfo { Code = "BR", Name = "Brazil", Lat = -14.24, Lng = -51.93 } },
            { "BS", new CountryGeoInfo { Code = "BS", Name = "Bahamas", Lat = 25.03, Lng = -77.40 } },
            { "BW", new CountryGeoInfo { Code = "BW", Name = "Botswana", Lat = -22.33, Lng = 24.68 } },
            { "BY", new CountryGeoInfo { Code = "BY", Name = "Belarus", Lat = 53.71, Lng = 27.95 } },
            { "BZ", new CountryGeoInfo { Code = "BZ", Name = "Belize", Lat = 17.19, Lng = -88.50 } },
            { "CA", new CountryGeoInfo { Code = "CA", Name = "Canada", Lat = 56.13, Lng = -106.35 } },
            { "CH", new CountryGeoInfo { Code = "CH", Name = "Switzerland", Lat = 46.82, Lng = 8.23 } },
            { "CL", new CountryGeoInfo { Code = "CL", Name = "Chile", Lat = -35.68, Lng = -71.54 } },
            { "CN", new CountryGeoInfo { Code = "CN", Name = "China", Lat = 35.86, Lng = 104.20 } },
            { "CO", new CountryGeoInfo { Code = "CO", Name = "Colombia", Lat = 4.57, Lng = -74.30 } },
            { "CR", new CountryGeoInfo { Code = "CR", Name = "Costa Rica", Lat = 9.75, Lng = -83.75 } },
            { "CU", new CountryGeoInfo { Code = "CU", Name = "Cuba", Lat = 21.52, Lng = -77.78 } },
            { "CY", new CountryGeoInfo { Code = "CY", Name = "Cyprus", Lat = 35.13, Lng = 33.43 } },
            { "CZ", new CountryGeoInfo { Code = "CZ", Name = "Czechia", Lat = 49.82, Lng = 15.47 } },
            { "DE", new CountryGeoInfo { Code = "DE", Name = "Germany", Lat = 51.17, Lng = 10.45 } },
            { "DK", new CountryGeoInfo { Code = "DK", Name = "Denmark", Lat = 56.26, Lng = 9.50 } },
            { "DO", new CountryGeoInfo { Code = "DO", Name = "Dominican Republic", Lat = 18.74, Lng = -70.16 } },
            { "DZ", new CountryGeoInfo { Code = "DZ", Name = "Algeria", Lat = 28.03, Lng = 1.66 } },
            { "EC", new CountryGeoInfo { Code = "EC", Name = "Ecuador", Lat = -1.83, Lng = -78.18 } },
            { "EE", new CountryGeoInfo { Code = "EE", Name = "Estonia", Lat = 58.60, Lng = 25.01 } },
            { "EG", new CountryGeoInfo { Code = "EG", Name = "Egypt", Lat = 26.82, Lng = 30.80 } },
            { "ES", new CountryGeoInfo { Code = "ES", Name = "Spain", Lat = 40.46, Lng = -3.75 } },
            { "FI", new CountryGeoInfo { Code = "FI", Name = "Finland", Lat = 61.92, Lng = 25.75 } },
            { "FJ", new CountryGeoInfo { Code = "FJ", Name = "Fiji", Lat = -17.71, Lng = 178.07 } },
            { "FR", new CountryGeoInfo { Code = "FR", Name = "France", Lat = 46.23, Lng = 2.21 } },
            { "GB", new CountryGeoInfo { Code = "GB", Name = "United Kingdom", Lat = 55.38, Lng = -3.44 } },
            { "GE", new CountryGeoInfo { Code = "GE", Name = "Georgia", Lat = 42.32, Lng = 43.36 } },
            { "GH", new CountryGeoInfo { Code = "GH", Name = "Ghana", Lat = 7.95, Lng = -1.02 } },
            { "GR", new CountryGeoInfo { Code = "GR", Name = "Greece", Lat = 39.07, Lng = 21.82 } },
            { "GT", new CountryGeoInfo { Code = "GT", Name = "Guatemala", Lat = 15.78, Lng = -90.23 } },
            { "HK", new CountryGeoInfo { Code = "HK", Name = "Hong Kong", Lat = 22.40, Lng = 114.11 } },
            { "HN", new CountryGeoInfo { Code = "HN", Name = "Honduras", Lat = 15.20, Lng = -86.24 } },
            { "HR", new CountryGeoInfo { Code = "HR", Name = "Croatia", Lat = 45.10, Lng = 15.20 } },
            { "HU", new CountryGeoInfo { Code = "HU", Name = "Hungary", Lat = 47.16, Lng = 19.50 } },
            { "ID", new CountryGeoInfo { Code = "ID", Name = "Indonesia", Lat = -0.79, Lng = 113.92 } },
            { "IE", new CountryGeoInfo { Code = "IE", Name = "Ireland", Lat = 53.41, Lng = -8.24 } },
            { "IL", new CountryGeoInfo { Code = "IL", Name = "Israel", Lat = 31.05, Lng = 34.85 } },
            { "IM", new CountryGeoInfo { Code = "IM", Name = "Isle of Man", Lat = 54.24, Lng = -4.55 } },
            { "IN", new CountryGeoInfo { Code = "IN", Name = "India", Lat = 20.59, Lng = 78.96 } },
            { "IQ", new CountryGeoInfo { Code = "IQ", Name = "Iraq", Lat = 33.22, Lng = 43.68 } },
            { "IR", new CountryGeoInfo { Code = "IR", Name = "Iran", Lat = 32.43, Lng = 53.69 } },
            { "IS", new CountryGeoInfo { Code = "IS", Name = "Iceland", Lat = 64.96, Lng = -19.02 } },
            { "IT", new CountryGeoInfo { Code = "IT", Name = "Italy", Lat = 41.87, Lng = 12.57 } },
            { "JE", new CountryGeoInfo { Code = "JE", Name = "Jersey", Lat = 49.21, Lng = -2.13 } },
            { "JM", new CountryGeoInfo { Code = "JM", Name = "Jamaica", Lat = 18.11, Lng = -77.30 } },
            { "JO", new CountryGeoInfo { Code = "JO", Name = "Jordan", Lat = 30.59, Lng = 36.24 } },
            { "JP", new CountryGeoInfo { Code = "JP", Name = "Japan", Lat = 36.20, Lng = 138.25 } },
            { "KE", new CountryGeoInfo { Code = "KE", Name = "Kenya", Lat = -0.02, Lng = 37.91 } },
            { "KR", new CountryGeoInfo { Code = "KR", Name = "South Korea", Lat = 35.91, Lng = 127.77 } },
            { "KW", new CountryGeoInfo { Code = "KW", Name = "Kuwait", Lat = 29.31, Lng = 47.48 } },
            { "KZ", new CountryGeoInfo { Code = "KZ", Name = "Kazakhstan", Lat = 48.02, Lng = 66.92 } },
            { "LB", new CountryGeoInfo { Code = "LB", Name = "Lebanon", Lat = 33.85, Lng = 35.86 } },
            { "LI", new CountryGeoInfo { Code = "LI", Name = "Liechtenstein", Lat = 47.17, Lng = 9.56 } },
            { "LK", new CountryGeoInfo { Code = "LK", Name = "Sri Lanka", Lat = 7.87, Lng = 80.77 } },
            { "LT", new CountryGeoInfo { Code = "LT", Name = "Lithuania", Lat = 55.17, Lng = 23.88 } },
            { "LU", new CountryGeoInfo { Code = "LU", Name = "Luxembourg", Lat = 49.82, Lng = 6.13 } },
            { "LV", new CountryGeoInfo { Code = "LV", Name = "Latvia", Lat = 56.88, Lng = 24.60 } },
            { "MA", new CountryGeoInfo { Code = "MA", Name = "Morocco", Lat = 31.79, Lng = -7.09 } },
            { "MC", new CountryGeoInfo { Code = "MC", Name = "Monaco", Lat = 43.75, Lng = 7.41 } },
            { "MD", new CountryGeoInfo { Code = "MD", Name = "Moldova", Lat = 47.41, Lng = 28.37 } },
            { "ME", new CountryGeoInfo { Code = "ME", Name = "Montenegro", Lat = 42.71, Lng = 19.37 } },
            { "MK", new CountryGeoInfo { Code = "MK", Name = "North Macedonia", Lat = 41.61, Lng = 21.75 } },
            { "MT", new CountryGeoInfo { Code = "MT", Name = "Malta", Lat = 35.94, Lng = 14.38 } },
            { "MU", new CountryGeoInfo { Code = "MU", Name = "Mauritius", Lat = -20.35, Lng = 57.55 } },
            { "MX", new CountryGeoInfo { Code = "MX", Name = "Mexico", Lat = 23.63, Lng = -102.55 } },
            { "MY", new CountryGeoInfo { Code = "MY", Name = "Malaysia", Lat = 4.21, Lng = 101.98 } },
            { "NG", new CountryGeoInfo { Code = "NG", Name = "Nigeria", Lat = 9.08, Lng = 8.68 } },
            { "NI", new CountryGeoInfo { Code = "NI", Name = "Nicaragua", Lat = 12.87, Lng = -85.21 } },
            { "NL", new CountryGeoInfo { Code = "NL", Name = "Netherlands", Lat = 52.13, Lng = 5.29 } },
            { "NO", new CountryGeoInfo { Code = "NO", Name = "Norway", Lat = 60.47, Lng = 8.47 } },
            { "NP", new CountryGeoInfo { Code = "NP", Name = "Nepal", Lat = 28.39, Lng = 84.12 } },
            { "NZ", new CountryGeoInfo { Code = "NZ", Name = "New Zealand", Lat = -40.90, Lng = 174.89 } },
            { "OM", new CountryGeoInfo { Code = "OM", Name = "Oman", Lat = 21.51, Lng = 55.92 } },
            { "PA", new CountryGeoInfo { Code = "PA", Name = "Panama", Lat = 8.54, Lng = -80.78 } },
            { "PE", new CountryGeoInfo { Code = "PE", Name = "Peru", Lat = -9.19, Lng = -75.02 } },
            { "PH", new CountryGeoInfo { Code = "PH", Name = "Philippines", Lat = 12.88, Lng = 121.77 } },
            { "PK", new CountryGeoInfo { Code = "PK", Name = "Pakistan", Lat = 30.38, Lng = 69.35 } },
            { "PL", new CountryGeoInfo { Code = "PL", Name = "Poland", Lat = 51.92, Lng = 19.15 } },
            { "PT", new CountryGeoInfo { Code = "PT", Name = "Portugal", Lat = 39.40, Lng = -8.22 } },
            { "PY", new CountryGeoInfo { Code = "PY", Name = "Paraguay", Lat = -23.44, Lng = -58.44 } },
            { "QA", new CountryGeoInfo { Code = "QA", Name = "Qatar", Lat = 25.35, Lng = 51.18 } },
            { "RO", new CountryGeoInfo { Code = "RO", Name = "Romania", Lat = 45.94, Lng = 24.97 } },
            { "RS", new CountryGeoInfo { Code = "RS", Name = "Serbia", Lat = 44.02, Lng = 21.01 } },
            { "RU", new CountryGeoInfo { Code = "RU", Name = "Russia", Lat = 61.52, Lng = 105.32 } },
            { "SA", new CountryGeoInfo { Code = "SA", Name = "Saudi Arabia", Lat = 23.89, Lng = 45.08 } },
            { "SE", new CountryGeoInfo { Code = "SE", Name = "Sweden", Lat = 60.13, Lng = 18.64 } },
            { "SG", new CountryGeoInfo { Code = "SG", Name = "Singapore", Lat = 1.35, Lng = 103.82 } },
            { "SI", new CountryGeoInfo { Code = "SI", Name = "Slovenia", Lat = 46.15, Lng = 14.99 } },
            { "SK", new CountryGeoInfo { Code = "SK", Name = "Slovakia", Lat = 48.67, Lng = 19.70 } },
            { "SM", new CountryGeoInfo { Code = "SM", Name = "San Marino", Lat = 43.94, Lng = 12.46 } },
            { "SN", new CountryGeoInfo { Code = "SN", Name = "Senegal", Lat = 14.50, Lng = -14.45 } },
            { "TH", new CountryGeoInfo { Code = "TH", Name = "Thailand", Lat = 15.87, Lng = 100.99 } },
            { "TN", new CountryGeoInfo { Code = "TN", Name = "Tunisia", Lat = 33.89, Lng = 9.54 } },
            { "TR", new CountryGeoInfo { Code = "TR", Name = "Turkey", Lat = 38.96, Lng = 35.24 } },
            { "TT", new CountryGeoInfo { Code = "TT", Name = "Trinidad and Tobago", Lat = 10.69, Lng = -61.22 } },
            { "TW", new CountryGeoInfo { Code = "TW", Name = "Taiwan", Lat = 23.70, Lng = 120.96 } },
            { "UA", new CountryGeoInfo { Code = "UA", Name = "Ukraine", Lat = 48.38, Lng = 31.17 } },
            { "US", new CountryGeoInfo { Code = "US", Name = "United States", Lat = 39.83, Lng = -98.58 } },
            { "UY", new CountryGeoInfo { Code = "UY", Name = "Uruguay", Lat = -32.52, Lng = -55.77 } },
            { "VE", new CountryGeoInfo { Code = "VE", Name = "Venezuela", Lat = 6.42, Lng = -66.59 } },
            { "VN", new CountryGeoInfo { Code = "VN", Name = "Vietnam", Lat = 14.06, Lng = 108.28 } },
            { "ZA", new CountryGeoInfo { Code = "ZA", Name = "South Africa", Lat = -30.56, Lng = 22.94 } },
            { "ZW", new CountryGeoInfo { Code = "ZW", Name = "Zimbabwe", Lat = -19.02, Lng = 29.15 } },
        };

        public static CountryGeoInfo Lookup(string rawCode)
        {
            if (string.IsNullOrWhiteSpace(rawCode)) return null;
            var code = rawCode.Trim();
            CountryGeoInfo info;
            return ByCode.TryGetValue(code, out info) ? info : null;
        }

        /// <summary>Builds a 🇬🇧-style flag from an ISO alpha-2 code (regional indicator symbols).</summary>
        public static string FlagEmoji(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != 2) return "🏳️";
            code = code.ToUpperInvariant();
            var c1 = char.ConvertFromUtf32(0x1F1E6 + (code[0] - 'A'));
            var c2 = char.ConvertFromUtf32(0x1F1E6 + (code[1] - 'A'));
            return c1 + c2;
        }
    }
}
