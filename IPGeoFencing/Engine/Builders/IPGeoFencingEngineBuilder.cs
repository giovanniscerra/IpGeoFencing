using IPGeoFencing.IP2Location.Abstractions;
using IPGeoFencing.IP2Location.Factories;

namespace IPGeoFencing.Engine.Builders
{
    public class IPGeoFencingEngineBuilder
    {
        public AreaBuilder AddIP2LocationProvider(IIP2LocationProvider ip2LocationProvider)
        {
            return AreaBuilder.Create(ip2LocationProvider);
        }

        public AreaBuilder AddIP2LocationFromCSVFile(string ip2LocationCSVFilePath)
        {
            IIP2LocationProvider ip2LocationProvider = IP2LocationProviderFactory.FromCSVFile(ip2LocationCSVFilePath);
            return AreaBuilder.Create(ip2LocationProvider);
        }
    }
}