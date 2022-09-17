using GeoCoordinatePortable;
using System.Collections;


namespace IPGeoFencing.GeographicAreas.DataModels
{
    public class PerimeterModel : IEnumerable<GeoCoordinate>
    {
        private readonly List<GeoCoordinate> _coordinates;

        public PerimeterModel()
        {
            _coordinates = new List<GeoCoordinate>();
        }

        public PerimeterModel(IEnumerable<GeoCoordinate> coordinates)
        {
            _coordinates = new List<GeoCoordinate>(coordinates);
        }

        public GeoCoordinate this[int index]
        {
            get { return _coordinates[index]; }
            set { _coordinates.Insert(index, value); }
        }

        public IEnumerator<GeoCoordinate> GetEnumerator()
        {
            return _coordinates.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
