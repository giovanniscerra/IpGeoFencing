using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.Providers;
using IPGeoFencing.IP2Location.Abstractions;
using IPGeoFencing.IP2Location.DataModels;
using System.Net;

namespace IPGeoFencing.Engine
{
    public class IPGeoFencingEngine
    {
        private readonly IIP2LocationProvider _ip2LocationProvider;
        private readonly IGeographicAreasProvider _geographicAreaProvider;
        private readonly IEnumerable<GeoFencingRule> _geofencingRules;
        private readonly IEnumerable<Action<IPAddress>> _defaultActions;

        public IPGeoFencingEngine(
                IIP2LocationProvider ip2LocationProvider,
                IGeographicAreasProvider geographicAreaProvider,
                IEnumerable<GeoFencingRule> geofencingRules,
                IEnumerable<Action<IPAddress>> defaultActions
            )
        {
            if (ip2LocationProvider is null)
                throw new ArgumentNullException($"{nameof(IPGeoFencingEngine)}->Ctor: {nameof(ip2LocationProvider)} parameter cannot be null");

            if (geographicAreaProvider is null)
                throw new ArgumentNullException($"{nameof(IPGeoFencingEngine)}->Ctor: {nameof(geographicAreaProvider)} parameter cannot be null");

            if (geofencingRules is null)
                throw new ArgumentNullException($"{nameof(IPGeoFencingEngine)}->Ctor: {nameof(geofencingRules)} parameter cannot be null");

            if (defaultActions is null)
                throw new ArgumentNullException($"{nameof(IPGeoFencingEngine)}->Ctor: {nameof(defaultActions)} parameter cannot be null");


            //Validate dupe rule names
            var dupeRuleNames = geofencingRules
                .GroupBy(g => g.Name, (key, g) => new { Key = key, Count = g.Count() })
                .Where(g=>g.Count > 1)
                .Select(g=>g.Key);

            if (dupeRuleNames.Any())
                throw new InvalidDataException($"{nameof(IPGeoFencingEngine)}->Ctor: {nameof(geofencingRules)} rules contain the following duplicate names: {string.Join(',', dupeRuleNames)}");

            _ip2LocationProvider = ip2LocationProvider;
            _geographicAreaProvider = geographicAreaProvider;
            _geofencingRules = geofencingRules;
            _defaultActions = defaultActions;
        }

        public void Run(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentException($"{nameof(IPGeoFencingEngine)}->Run: {nameof(ipAddress)} parameter must be specified");

            IPAddress? parsedIPAddress;
            if (!IPAddress.TryParse(ipAddress, out parsedIPAddress))
                throw new ArgumentException($"{nameof(IPGeoFencingEngine)}->Run: Invalid IP address: {ipAddress}");

            Run(parsedIPAddress);
        }

        public void Run(IPAddress ipAddress)
        {
            if (ipAddress is null)
                throw new ArgumentNullException($"{nameof(IPGeoFencingEngine)}->Run: {nameof(ipAddress)} parameter cannot be null");

            LocationModel? ipLocation = _ip2LocationProvider.GetLocationFromIP(ipAddress);

            if (ipLocation is null)
                throw new InvalidDataException($"{nameof(IPGeoFencingEngine)}->Run: Unable to the find geographic location of the IP address {ipAddress}");

            var location = new GeoCoordinate(ipLocation.Latitude, ipLocation.Longitude);

            var matchingGeographicAreas = _geographicAreaProvider.GetAreasContaining(location);

            var matchingRules = _geofencingRules.Where(r => r.IsMatch(matchingGeographicAreas, ipAddress, ipLocation));

            if (matchingRules.Any())
            {
                //Executing all matching rules
                foreach (var matchingRule in matchingRules)
                {
                    matchingRule.Execute(matchingGeographicAreas, ipAddress, ipLocation);
                }
            }
            else
            {
                //No rule for given IP, running default actions
                foreach (var defaultAction in _defaultActions)
                {
                    defaultAction(ipAddress);
                }
            }
        }
    }
}
