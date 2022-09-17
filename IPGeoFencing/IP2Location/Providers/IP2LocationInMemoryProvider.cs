using IPGeoFencing.IP2Location.DataModels;
using IPGeoFencing.IP2Location.Abstractions;
using System.Net;

namespace IPGeoFencing.IP2Location.Providers
{
    public class IP2LocationInMemoryProvider : IP2LocationProvider, IIP2LocationProvider
    {
        private readonly IEnumerable<IPLocationModel> _ip2Locations;

        public IP2LocationInMemoryProvider(IEnumerable<IPLocationModel> ip2Locations)
        {
            if (ip2Locations is null)
                throw new ArgumentNullException($"{nameof(IP2LocationInMemoryProvider)}->Ctor: {nameof(ip2Locations)} ctor parameter canot be null");

            _ip2Locations = ip2Locations;
        }

        public override LocationModel? GetLocationFromIP(long ipAddress)
        {
            LocationModel? location = null;

            try
            {
                location = _ip2Locations.First(iploc => ipAddress >= iploc.IPFrom && ipAddress <= iploc.IPTo);
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"{nameof(IP2LocationInMemoryProvider)}->GetLocationFromIP: Unable to find location for IP: {new IPAddress(ipAddress)}{Environment.NewLine}{ex}{Environment.NewLine}");
            }

            return location;
        }
    }
}