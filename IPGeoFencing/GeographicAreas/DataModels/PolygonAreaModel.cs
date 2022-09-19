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

            GeoCoordinate[] polygonPointsWithClosure = PolygonPointsWithClosure(coordinates.ToArray());

            int windingNumber = 0;

            for (int pointIndex = 0; pointIndex < polygonPointsWithClosure.Length - 1; pointIndex++)
            {
                Edge edge = new Edge(polygonPointsWithClosure[pointIndex], polygonPointsWithClosure[pointIndex + 1]);
                windingNumber += AscendingIntersection(location, edge);
                windingNumber -= DescendingIntersection(location, edge);
            }

            return windingNumber != 0;
        }

        private GeoCoordinate[] PolygonPointsWithClosure(GeoCoordinate[] coordinates)
        {
            // _points should remain immutable, thus creation of a closed point set (starting point repeated)
            return new List<GeoCoordinate>(coordinates)
            {
                new GeoCoordinate(coordinates[0].Latitude, coordinates[0].Longitude)
            }.ToArray();
        }

        private static int AscendingIntersection(GeoCoordinate location, Edge edge)
        {
            return Wind(location, edge, Position.Left);
        }

        private static int DescendingIntersection(GeoCoordinate location, Edge edge)
        {
            return Wind(location, edge, Position.Right);
        }

        private static int Wind(GeoCoordinate location, Edge edge, Position position)
        {
            if (edge.RelativePositionOf(location) != position) { return 0; }

            return 1;
        }

        private class Edge
        {
            private readonly GeoCoordinate _startPoint;
            private readonly GeoCoordinate _endPoint;

            public Edge(GeoCoordinate startPoint, GeoCoordinate endPoint)
            {
                _startPoint = startPoint;
                _endPoint = endPoint;
            }

            public Position RelativePositionOf(GeoCoordinate location)
            {
                double positionCalculation =
                    (_endPoint.Longitude - _startPoint.Longitude) * (location.Latitude - _startPoint.Latitude) -
                    (location.Longitude - _startPoint.Longitude) * (_endPoint.Latitude - _startPoint.Latitude);

                if (positionCalculation > 0) { return Position.Left; }

                if (positionCalculation < 0) { return Position.Right; }

                return Position.Center;
            }
        }

        private enum Position
        {
            Left,
            Right,
            Center
        }
    }
}