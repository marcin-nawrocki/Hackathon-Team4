using System.Linq;

namespace Supercontrol.Web.Dashboard.Helpers
{
    public static class CountryFlag
    {
        public static string CodeFor(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return null;
            }

            var trimmed = country.Trim();
            if (trimmed.Length == 2 && trimmed.All(char.IsLetter))
            {
                return trimmed.ToLowerInvariant();
            }

            return null;
        }

        public static string UrlFor(string country)
        {
            var code = CodeFor(country);
            return code == null ? null : "https://flagcdn.io/flags/4x3/" + code + ".svg";
        }
    }
}
