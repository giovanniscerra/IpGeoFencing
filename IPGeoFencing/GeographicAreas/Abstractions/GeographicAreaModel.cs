using GeoCoordinatePortable;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using IPGeoFencing.GeographicAreas.DataModels;

namespace IPGeoFencing.GeographicAreas.Abstractions
{
    public abstract class GeographicAreaModel : IGeographicAreaModel
    {
        public GeographicAreaModel(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(GeographicAreaModel)}->Ctor: {nameof(name)} ctor parameter must be specified");

            Name = name;
        }

        public string? Name { get; private set; }

        public abstract bool Contains(GeoCoordinate point);

        public static GeographicAreaModel FromGeoJSON(Feature feature)
        {
            if (feature is null)
                throw new InvalidDataException($"{nameof(GeographicAreaModel)}->FromGeoJSON: parameter {nameof(feature)} cannot be null");

            if (!feature.Properties.ContainsKey("name"))
                throw new InvalidDataException($"{nameof(GeographicAreaModel)}->FromGeoJSON: Property name is missing from feature {feature} of type {feature.Geometry.Type}");

            string name = feature.Properties["name"]?.ToString() ?? string.Empty;

            if (feature.Geometry is not null
                    && (feature.Geometry.Type == GeoJSONObjectType.Polygon
                        || feature.Geometry.Type == GeoJSONObjectType.MultiPolygon
                        || feature.Geometry.Type == GeoJSONObjectType.Point))
            {
                if (feature.Geometry is Polygon)
                {
                    var polygon = feature.Geometry as Polygon;

                    if (polygon is not null && polygon.Coordinates.Any())
                    {
                        var perimeters = GetPerimeters(polygon.Coordinates);
                        return new PolygonAreaModel(name, perimeters);
                    }
                }

                if (feature.Geometry is MultiPolygon)
                {
                    var multyPolygon = feature.Geometry as MultiPolygon;

                    List<PolygonAreaModel> polygons = new List<PolygonAreaModel>();

                    if (multyPolygon is not null)
                        foreach (var polygon in multyPolygon.Coordinates)
                            if (polygon is not null && polygon.Coordinates.Any())
                            {
                                var perimeters = GetPerimeters(polygon.Coordinates);
                                polygons.Add(new PolygonAreaModel(name, perimeters));
                            }

                    return new MultiPolygonAreaModel(name, polygons);
                }

                if (feature.Geometry is Point)
                {
                    double radiusInMeters = 0;

                    var point = feature.Geometry as Point;

                    if (!feature.Properties.ContainsKey("radius")
                        || feature.Properties["radius"] is null
                        || !double.TryParse(feature.Properties["radius"].ToString(), out radiusInMeters))
                        radiusInMeters = 0;

                    if (point is not null)
                        return new CircleAreaModel(
                                        name,
                                        new GeoCoordinate(point.Coordinates.Latitude, point.Coordinates.Longitude),
                                        radiusInMeters);
                }
            }

            throw new InvalidDataException($"{nameof(GeographicAreaModel)}->FromGeoJSON: Unsupported feature geometry type {feature?.Geometry?.Type}. Only Polygon, Mulpoligon and Point Geometry types are supported");
        }

        private static IEnumerable<PerimeterModel> GetPerimeters(IEnumerable<LineString> coordinates)
        {
            return coordinates
                     .Select(l => new PerimeterModel(
                         l.Coordinates
                             .Distinct()
                             .Select(p => new GeoCoordinate(p.Latitude, p.Longitude))));
        }
    }
}