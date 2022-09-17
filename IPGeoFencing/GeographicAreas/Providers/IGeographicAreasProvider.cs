using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.Abstractions;

namespace IPGeoFencing.GeographicAreas.Providers
{
    public interface IGeographicAreasProvider
    {
        IEnumerable<GeographicAreaModel> GetAreasContaining(GeoCoordinate point);
    }
}