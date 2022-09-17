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

        public GeoCoordinate ToGeoCoordinate()
        {
            return new GeoCoordinate(Latitude, Longitude);
        }
    }
}
