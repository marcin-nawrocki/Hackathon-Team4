using System.Collections.Generic;
using System.Web.Script.Serialization;
using Supercontrol.Web.Dashboard.Models;

namespace Supercontrol.Web.Dashboard.Helpers
{
    public static class TopLocationsMapBuilder
    {
        private const int HighVolumeRankThreshold = 3;

        public static string ToGeoJson(IReadOnlyList<TopLocationDto> locations)
        {
            var features = new List<object>();

            for (var i = 0; i < locations.Count; i++)
            {
                var location = locations[i];
                var country = string.IsNullOrWhiteSpace(location.CustomerCountry)
                    ? "Unknown"
                    : location.CustomerCountry.Trim();

                if (string.Equals(country, "Unknown", System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var coords = CountryCentroidLookup.TryGet(country);
                if (coords == null)
                {
                    continue;
                }

                features.Add(new
                {
                    type = "Feature",
                    properties = new
                    {
                        country = country,
                        bookings = location.C,
                        rank = i + 1,
                        score = i < HighVolumeRankThreshold ? "high" : "low"
                    },
                    geometry = new
                    {
                        type = "Point",
                        coordinates = coords
                    }
                });
            }

            var collection = new
            {
                type = "FeatureCollection",
                features = features
            };

            return new JavaScriptSerializer().Serialize(collection);
        }
    }
}
