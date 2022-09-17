using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.Abstractions;
using IPGeoFencing.IP2Location.DataModels;
using System.Net;

namespace IPGeoFencing.Engine
{
    public class GeoFencingRule: IEquatable<GeoFencingRule>
    {
        private readonly string _ruleName;
        private readonly Func<IEnumerable<GeographicAreaModel>, IPAddress, LocationModel, bool> _predicate;
        private readonly Action<IEnumerable<GeographicAreaModel>, IPAddress, LocationModel> _action;

        public GeoFencingRule(
                string ruleName,
                Func<IEnumerable<GeographicAreaModel>, IPAddress, LocationModel, bool> predicate,
                Action<IEnumerable<GeographicAreaModel>, IPAddress, LocationModel> action
            )
        {
            if (string.IsNullOrWhiteSpace(ruleName))
                throw new ArgumentNullException($"{nameof(IPGeoFencingEngine)}->Ctor: {nameof(_ruleName)} parameter must be specified");

            if (predicate is null)
                throw new ArgumentNullException($"{nameof(IPGeoFencingEngine)}->Ctor: {nameof(predicate)} parameter cannot be null");

            if (action is null)
                throw new ArgumentNullException($"{nameof(IPGeoFencingEngine)}->Ctor: {nameof(action)} parameter cannot be null");

            _ruleName = ruleName.Trim();
            _predicate = predicate;
            _action = action;
        }

        public string Name { get { return _ruleName; } }

        public bool IsMatch(IEnumerable<GeographicAreaModel> matchingGeographicArea, IPAddress ipAddress, LocationModel location)
        {
            return _predicate(matchingGeographicArea, ipAddress, location);
        }

        public void Execute(IEnumerable<GeographicAreaModel> matchingGeographicArea, IPAddress ipAddress, LocationModel location)
        {
            _action(matchingGeographicArea, ipAddress, location);
        }

        public static bool operator ==(GeoFencingRule a, GeoFencingRule b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (((object?)a is null) || ((object?)b is null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(GeoFencingRule geoFencingRuleA, GeoFencingRule geoFencingRuleB)
        {
            return !(geoFencingRuleA == geoFencingRuleB);
        }

        public bool Equals(GeoFencingRule? geoFencingRule)
        {
            return geoFencingRule is not null 
                    && _ruleName == geoFencingRule.Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as GeoFencingRule);
        }

        public override int GetHashCode()
        {
            return _ruleName.GetHashCode();
        }
    }
}