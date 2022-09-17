using IPGeoFencing.IP2Location.DataModels;
using System.Net;

namespace IPGeoFencing.IP2Location.Abstractions
{
    public interface IIP2LocationProvider
    {
        LocationModel? GetLocationFromIP(IPAddress ipAddress);
        LocationModel? GetLocationFromIP(long ipAddress);
    }
}