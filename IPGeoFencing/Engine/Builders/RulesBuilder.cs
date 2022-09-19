using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.Abstractions;
using IPGeoFencing.GeographicAreas.Providers;
using IPGeoFencing.IP2Location.Abstractions;
using IPGeoFencing.IP2Location.DataModels;
using System.Net;

namespace IPGeoFencing.Engine.Builders
{
    public class RulesBuilder
    {
        private readonly IIP2LocationProvider _ip2LocationProvider;
        private readonly IGeographicAreasProvider _geographicAreaProvider;
        private readonly List<GeoFencingRule> _geoFencingRules = new List<GeoFencingRule>();
        private readonly List<Action<IPAddress, LocationModel>> _defaultActions = new List<Action<IPAddress, LocationModel>>();

        public RulesBuilder(IIP2LocationProvider ip2LocationProvider, IGeographicAreasProvider geographicAreaProvider)
        {
            if (ip2LocationProvider is null)
                throw new ArgumentNullException($"{nameof(RulesBuilder)}->Ctor: {nameof(ip2LocationProvider)} parameter cannit be null");

            if (geographicAreaProvider is null)
                throw new ArgumentNullException($"{nameof(RulesBuilder)}->Ctor: {nameof(geographicAreaProvider)} parameter cannit be null");

            _ip2LocationProvider = ip2LocationProvider;
            _geographicAreaProvider = geographicAreaProvider;
        }

        public RulesBuilder AddRule(
                     string name,    
                     Func<IEnumerable<GeographicAreaModel>, IPAddress, LocationModel, bool> predicate,
                     Action<IEnumerable<GeographicAreaModel>, IPAddress, LocationModel> action)
        {
            if (predicate is null)
                throw new ArgumentNullException($"{nameof(RulesBuilder)}->AddRule: {nameof(predicate)} parameter cannot be null");

            if (action is null)
                throw new ArgumentNullException($"{nameof(RulesBuilder)}->AddRule: {nameof(action)} parameter cannot be null");


            _geoFencingRules.Add(new GeoFencingRule(name, predicate, action));

            return this;
        }

        public RulesBuilder AddRule(GeoFencingRule geoFencingRule)
        {
            if (geoFencingRule is null)
                throw new ArgumentNullException($"{nameof(RulesBuilder)}->AddRule: {nameof(geoFencingRule)} parameter cannot be null");

            _geoFencingRules.Add(geoFencingRule);

            return this;
        }

        public RulesBuilder AddDefaultAction(Action<IPAddress, LocationModel> defaultAction)
        {
            if (defaultAction is null)
                throw new ArgumentNullException($"{nameof(RulesBuilder)}->AddDefaultAction: {nameof(defaultAction)} parameter cannot be null");

            _defaultActions.Add(defaultAction);

            return this;
        }

        public IPGeoFencingEngine Build()
        {
            return new IPGeoFencingEngine(
                            _ip2LocationProvider,
                            _geographicAreaProvider,
                            _geoFencingRules,
                            _defaultActions);
        }

        internal static RulesBuilder Create(IIP2LocationProvider ip2LocationProvider, IGeographicAreasProvider geographicAreaProvider)
        {
            return new RulesBuilder(ip2LocationProvider, geographicAreaProvider);
        }
    }
}
