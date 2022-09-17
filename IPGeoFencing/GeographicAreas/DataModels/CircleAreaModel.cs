using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.Abstractions;

namespace IPGeoFencing.GeographicAreas.DataModels
{
    public class CircleAreaModel: GeographicAreaModel, IGeographicAreaModel
    {

        public readonly GeoCoordinate _center;

        public readonly double _radiusInMeters;

        public CircleAreaModel(string name, GeoCoordinate center, double radiusInMeters):
            base(name)
        {
            if (center is null)
                throw new ArgumentNullException($"{nameof(CircleAreaModel)}->Ctor: {nameof(center)} ctor parameter canot be null");

            if (radiusInMeters < 0)
                throw new ArgumentOutOfRangeException($"{nameof(CircleAreaModel)}->Ctor: {nameof(radiusInMeters)} cannot be negative");


            _center = center;
            _radiusInMeters = radiusInMeters;
        }

        public override bool Contains(GeoCoordinate point)
        {
            if (_center.Equals(point))
                return true;

            return _center.GetDistanceTo(point) <= _radiusInMeters;
        }
    }
}