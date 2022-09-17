using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.Abstractions;

namespace IPGeoFencing.GeographicAreas.DataModels
{
    public class PolygonAreaModel : GeographicAreaModel, IGeographicAreaModel
    {
        private readonly IEnumerable<GeoCoordinate> _outerPerimeter;
        private readonly IEnumerable<IEnumerable<GeoCoordinate>> _innerPerimeters; //holes

        public PolygonAreaModel(string name, IEnumerable<PerimeterModel> perimeters) :
            base(name)
        {
            if (perimeters is null)
                throw new ArgumentNullException($"{nameof(PolygonAreaModel)}->Ctor: {nameof(perimeters)} ctor parameter canot be null");

            if (!perimeters.Any())
                throw new ArgumentNullException($"{nameof(PolygonAreaModel)}->Ctor: {nameof(perimeters)} ctor parameter must have at least one perimeter");

            foreach (var perimeter in perimeters)
                if (perimeter.Distinct().Count() <= 2)
                    throw new ArgumentNullException($"{nameof(PolygonAreaModel)}->Ctor: {nameof(perimeters)} ctor parameter must contain only permieters that have at least 3 distinct points");

            _outerPerimeter = perimeters.First();
            _innerPerimeters = perimeters.Skip(1);
        }

        public override bool Contains(GeoCoordinate location)
        {
            if (!IsLocationInPoligon(this.Name, _outerPerimeter, location))
                return false;

            //Location is in polygon, need to exclude holes (if any)
            foreach (var innerPerimeter in _innerPerimeters)
                if (IsLocationInPoligon(this.Name, innerPerimeter, location))
                    return false;

            return true;
        }

        private bool IsLocationInPoligon(string? name, IEnumerable<GeoCoordinate> coordinates, GeoCoordinate location)
        {
            //If the bounding box does not contain the location, no point in calculating the winding number
            var boundingBox = new BoundingBoxAreaModel(name, coordinates);
            if (!boundingBox.Contains(location))
                return false;

            /// Winding Number algorithm ported from C++ to C# by Manuel Castro:
            /// https://stackoverflow.com/questions/924171/geo-fencing-point-inside-outside-polygon
            int n = coordinates.Count();

            List<GeoCoordinate> poly = new List<GeoCoordinate>(_outerPerimeter);
            poly.Add(new GeoCoordinate(poly[0].Latitude, poly[0].Longitude));

            GeoCoordinate[] v = poly.ToArray();

            int wn = 0;    // the winding number counter

            // loop through all edges of the polygon
            for (int i = 0; i < n; i++)
            {   // edge from V[i] to V[i+1]
                if (v[i].Latitude <= location.Latitude)
                {         // start y <= P.y
                    if (v[i + 1].Latitude > location.Latitude)      // an upward crossing
                        if (isLeft(v[i], v[i + 1], location) > 0)  // P left of edge
                            ++wn;            // have a valid up intersect
                }
                else
                {                       // start y > P.y (no test needed)
                    if (v[i + 1].Latitude <= location.Latitude)     // a downward crossing
                        if (isLeft(v[i], v[i + 1], location) < 0)  // P right of edge
                            --wn;            // have a valid down intersect
                }
            }
            if (wn != 0)
                return true;
            else
                return false;
        }


        private static int isLeft(GeoCoordinate P0, GeoCoordinate P1, GeoCoordinate P2)
        {
            double calc = ((P1.Longitude - P0.Longitude) * (P2.Latitude - P0.Latitude)
                    - (P2.Longitude - P0.Longitude) * (P1.Latitude - P0.Latitude));
            if (calc > 0)
                return 1;
            else if (calc < 0)
                return -1;
            else
                return 0;
        }
    }
}