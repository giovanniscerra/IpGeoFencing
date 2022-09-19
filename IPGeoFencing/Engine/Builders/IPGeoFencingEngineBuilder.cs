using IPGeoFencing.IP2Location.Abstractions;
using IPGeoFencing.IP2Location.Factories;

namespace IPGeoFencing.Engine.Builders
{
    public class IPGeoFencingEngineBuilder
    {
        public AreasBuilder AddIP2LocationProvider(IIP2LocationProvider ip2LocationProvider)
        {
            return AreasBuilder.Create(ip2LocationProvider);
        }

        public AreasBuilder AddIP2LocationFromCSVFile(string ip2LocationCSVFilePath)
        {
            IIP2LocationProvider ip2LocationProvider = IP2LocationProviderFactory.FromCSVFile(ip2LocationCSVFilePath);
            return AreasBuilder.Create(ip2LocationProvider);
        }
    }
}