using IPGeoFencing.IP2Location.DataModels;
using IPGeoFencing.Networking;
using System.Net;

namespace IPGeoFencing.IP2Location.Abstractions
{
    public abstract class IP2LocationProvider
    {
        public LocationModel? GetLocationFromIP(IPAddress ipAddress)
        {
            if (ipAddress is null)
                throw new ArgumentNullException($"{nameof(IP2LocationProvider)}->GetLocationFromIP: {nameof(ipAddress)} parameter cannot be null");

            long ipNumber;

            try
            {
                ipNumber = NetworkUtils.IPToNumber(ipAddress);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"{nameof(IP2LocationProvider)}->GetLocationFromIP: Unable to convert IP address {ipAddress} to number: {ex}");
            }

            return GetLocationFromIP(ipNumber);
        }

        public abstract LocationModel? GetLocationFromIP(long ipAddress);
    }
}