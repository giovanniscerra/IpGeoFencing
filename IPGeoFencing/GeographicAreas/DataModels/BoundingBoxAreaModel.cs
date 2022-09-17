using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.Abstractions;

namespace IPGeoFencing.GeographicAreas.DataModels
{
    public class BoundingBoxAreaModel : GeographicAreaModel, IGeographicAreaModel
    {
        private readonly double minLat;
        private readonly double minLon;
        private readonly double maxLat;
        private readonly double maxLon;

        public BoundingBoxAreaModel(string? name, IEnumerable<GeoCoordinate> coordinates):
            base(name)
        {
            if (coordinates is null)
                throw new ArgumentNullException($"{nameof(BoundingBoxAreaModel)}->Ctor: {nameof(coordinates)} ctor parameter canot be null");

            if (coordinates.Distinct().Count() <= 2)
                throw new ArgumentNullException($"{nameof(BoundingBoxAreaModel)}->Ctor: {nameof(coordinates)} ctor parameter must have at least 3 distinct locations");

            minLat = coordinates.Min(c => c.Latitude);
            minLon = coordinates.Min(c => c.Longitude);
            maxLat = coordinates.Max(c => c.Latitude);
            maxLon = coordinates.Max(c => c.Longitude);
        }
        public override bool Contains(GeoCoordinate point)
        {
            return (point.Latitude >= minLat
                 && point.Latitude <= maxLat
                 && point.Longitude >= minLon
                 && point.Longitude <= maxLon);
        }
    }
}
