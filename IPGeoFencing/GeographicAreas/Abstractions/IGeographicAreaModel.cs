using GeoCoordinatePortable;

namespace IPGeoFencing.GeographicAreas.Abstractions
{
    public interface IGeographicAreaModel
    {
        string? Name { get; }

        bool Contains(GeoCoordinate point);
    }
}