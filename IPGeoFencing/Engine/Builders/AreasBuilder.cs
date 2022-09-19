using IPGeoFencing.GeographicAreas.Factories;
using IPGeoFencing.GeographicAreas.Providers;
using IPGeoFencing.IP2Location.Abstractions;

namespace IPGeoFencing.Engine.Builders
{
    public class AreasBuilder
    {
        private readonly IIP2LocationProvider _ip2LocationProvider;

        private AreasBuilder(IIP2LocationProvider ip2LocationProvider)
        {
            if (ip2LocationProvider is null)
                throw new ArgumentNullException($"{nameof(AreasBuilder)}->Ctor: {nameof(ip2LocationProvider)} parameter cannit be null");

            _ip2LocationProvider = ip2LocationProvider;
        }

        public RulesBuilder AddGeographicAreasProvider(IGeographicAreasProvider geographicAreasProvider)
        {
            return RulesBuilder.Create(_ip2LocationProvider, geographicAreasProvider);
        }

        public RulesBuilder AddGeographicAreasFromGeoJSONFile(string geoJSONFilePath)
        {
            var geographicAreasProvider = GeographicAreasProviderFactory.FromGeoJSONFile(geoJSONFilePath);

            return RulesBuilder.Create(_ip2LocationProvider, geographicAreasProvider);
        }

        internal static AreasBuilder Create(IIP2LocationProvider ip2LocationProvider)
        {
            return new AreasBuilder(ip2LocationProvider);
        }
    }
}
