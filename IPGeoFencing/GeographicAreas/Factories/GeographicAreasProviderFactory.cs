using GeoJSON.Net;
using GeoJSON.Net.Feature;
using IPGeoFencing.GeographicAreas.Abstractions;
using IPGeoFencing.GeographicAreas.Providers;
using Newtonsoft.Json;

namespace IPGeoFencing.GeographicAreas.Factories
{
    public static class GeographicAreasProviderFactory
    {
        public async static Task<IGeographicAreasProvider> FromGeoJSONFileAsync(string geoJSONFilePath)
        {
            ValidateFilePath(geoJSONFilePath);

            var jsonText = await File.ReadAllTextAsync(geoJSONFilePath);

            return GetGeographicAreasInMemoryProvider(jsonText);
        }

        public static IGeographicAreasProvider FromGeoJSONFile(string geoJSONFilePath)
        {
            ValidateFilePath(geoJSONFilePath);

            var jsonText = File.ReadAllText(geoJSONFilePath);

            return GetGeographicAreasInMemoryProvider(jsonText);
        }

        private static void ValidateFilePath(string geoJSONFilePath)
        {
            if (geoJSONFilePath == null)
                throw new ArgumentNullException($"{nameof(GeographicAreasProviderFactory)}->CreateInMemoryProviderFromCSVFile: {nameof(geoJSONFilePath)} parameter canot be null");

            if (!File.Exists(geoJSONFilePath))
                throw new FileNotFoundException($"{nameof(GeographicAreasProviderFactory)}->CreateInMemoryProviderFromCSVFile: Unable to locate GeoJSON file {geoJSONFilePath}");

        }

        private static IGeographicAreasProvider GetGeographicAreasInMemoryProvider(string jsonText)
        {
            var geoJsonObject = JsonConvert.DeserializeObject<FeatureCollection>(jsonText);

            var geographicAreas = geoJsonObject.Features
                        .Where(f => f.Geometry is not null
                                    && (f.Geometry.Type == GeoJSONObjectType.Polygon
                                        || f.Geometry.Type == GeoJSONObjectType.MultiPolygon
                                        || f.Geometry.Type == GeoJSONObjectType.Point))
                        .Select(f => GeographicAreaModel.FromGeoJSON(f));

            return new GeographicAreasInMemoryProvider(geographicAreas);
        }
    }
}
