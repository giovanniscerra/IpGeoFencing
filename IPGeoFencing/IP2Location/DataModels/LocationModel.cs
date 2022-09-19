using GeoCoordinatePortable;

namespace IPGeoFencing.IP2Location.DataModels
{
    public class LocationModel
    {
        public string? CountryCode { get; protected set; }
        public string? CountryName { get; protected set; }
        public string? RegionName { get; protected set; }
        public string? CityName { get; protected set; }
        public double Latitude { get; protected set; }
        public double Longitude { get; protected set; }
        public string? ZipCode { get; protected set; }
        public string? TimeZone { get; protected set; }

        public static LocationModel Create(
                string countryCode = "",
                string countryName = "",
                string regionName = "",
                string cityName = "",
                double latitude = 0,
                double longitude = 0,
                string zipCode = "",
                string timeZone = "")
        {

            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException($"{nameof(latitude)} must be between -90 and 90");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException($"{nameof(longitude)} must be between -180 and 180");

            var location = new LocationModel
            {
                CountryCode = countryCode,
                CountryName = countryName,
                RegionName = regionName,
                CityName = cityName,
                Latitude = latitude,
                Longitude = longitude,
                ZipCode = zipCode,
                TimeZone = timeZone,
            };

            return location;
        }

        public GeoCoordinate ToGeoCoordinate()
        {
            return new GeoCoordinate(Latitude, Longitude);
        }
    }
}
