using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.Abstractions;

namespace IPGeoFencing.GeographicAreas.DataModels
{
    public class MultiPolygonAreaModel : GeographicAreaModel, IGeographicAreaModel
    {
        private readonly IEnumerable<PolygonAreaModel> _poligons;

        public MultiPolygonAreaModel(string name, IEnumerable<PolygonAreaModel> poligons):
            base(name)
        {
            if (poligons is null)
                throw new ArgumentNullException($"{nameof(MultiPolygonAreaModel)}->Ctor: {nameof(poligons)} ctor parameter canot be null");


            _poligons = poligons;
        }

        public override bool Contains(GeoCoordinate point)
        {
            return _poligons.Any(p => p.Contains(point));
        }
    }
}