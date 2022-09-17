using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.Abstractions;

namespace IPGeoFencing.GeographicAreas.Providers
{
    public class GeographicAreasInMemoryProvider : IGeographicAreasProvider
    {
        private readonly IEnumerable<GeographicAreaModel> _geographicAreas;

        public GeographicAreasInMemoryProvider(IEnumerable<GeographicAreaModel> geographicAreas)
        {
            if (geographicAreas is null)
                throw new ArgumentNullException($"{nameof(GeographicAreasInMemoryProvider)}->Ctor: {nameof(geographicAreas)} parameter cannot be null");

            _geographicAreas = geographicAreas;
        }

        public IEnumerable<GeographicAreaModel> GetAreasContaining(GeoCoordinate point)
        {
            return _geographicAreas.Where(g => g.Contains(point));
        }
    }
}
