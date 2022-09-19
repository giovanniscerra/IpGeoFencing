using GeoCoordinatePortable;
using IPGeoFencing.Engine;
using IPGeoFencing.GeographicAreas.Abstractions;
using IPGeoFencing.GeographicAreas.DataModels;
using IPGeoFencing.GeographicAreas.Providers;
using IPGeoFencing.IP2Location.Abstractions;
using IPGeoFencing.IP2Location.DataModels;
using Moq;
using System.Net;


namespace IPGeoFencing.UnitTests.Engine
{
    [TestClass]
    public class IPGeoFencingEngineTest
    {

        private IEnumerable<GeographicAreaModel>? _actionAreas;
        private IPAddress? _actionIp;
        private LocationModel? _actionLocation;

        private IPAddress? _defaultActionIp;

        public IIP2LocationProvider GetTestIP2LocationProvider()
        {
            var ip2LocationproviderMock = new Mock<IIP2LocationProvider>();
            ip2LocationproviderMock
                .Setup(provider => provider.GetLocationFromIP(It.IsAny<IPAddress>()))
                .Returns(LocationModel.Create(latitude: 0,
                                              longitude: 0,
                                              regionName: "testregion"));

            return ip2LocationproviderMock.Object;
        }

        public IGeographicAreasProvider GetTestGeographicAreasProvider()
        {
            var geographicAreaProviderMock = new Mock<IGeographicAreasProvider>();
            geographicAreaProviderMock
                .Setup(provider => provider.GetAreasContaining(It.IsAny<GeoCoordinate>()))
                .Returns(new GeographicAreaModel[] { new CircleAreaModel("testarea", new GeoCoordinate(0,0), 100) });

            return geographicAreaProviderMock.Object;
        }


        public IEnumerable<GeoFencingRule> GetAlwaysMatchingGeofencingRules()
        {
            var rule = new GeoFencingRule("testrule",
             (areas, ip, loc) => true,
             (areas, ip, loc) =>
             {
                 _actionAreas = areas;
                 _actionIp = ip;
                 _actionLocation = loc;
             });

            return new GeoFencingRule[] { rule };
        }

        public IEnumerable<GeoFencingRule> GetNeverMatchingGeofencingRules()
        {
            var rule = new GeoFencingRule("testrule",
             (areas, ip, loc) => false,
             (areas, ip, loc) =>
             {
                 _actionAreas = areas;
                 _actionIp = ip;
                 _actionLocation = loc;
             });

            return new GeoFencingRule[] { rule };
        }

        public IEnumerable<Action<IPAddress, LocationModel>> GetTestDefaultActions()
        {
            return new Action<IPAddress, LocationModel>[] { (ip, loc) => { _defaultActionIp = ip; } };
        }

        [TestInitialize]
        public void TestInit()
        {
            _actionAreas = null;
            _actionIp = null;
            _actionLocation = null;
            _defaultActionIp = null;
        }

        [TestMethod]
        public void TestIPGeoFencingEngine_ShouldFireAction()
        {
            var ip2Locationprovider = GetTestIP2LocationProvider();
            var geographicAreaProvider = GetTestGeographicAreasProvider();
            var alwaysMatchingRules = GetAlwaysMatchingGeofencingRules();
            var defaultActions = GetTestDefaultActions();

            var engine = new IPGeoFencingEngine(
                                    ip2Locationprovider,
                                    geographicAreaProvider,
                                    alwaysMatchingRules,
                                    defaultActions);

            engine.Run("10.10.10.10");

            Assert.IsNull(_defaultActionIp);

            Assert.IsNotNull(_actionAreas);
            Assert.IsNotNull(_actionIp);
            Assert.IsNotNull(_actionLocation);

            Assert.AreEqual(_actionAreas.Count(), 1);
            Assert.AreEqual(_actionAreas.First().Name, "testarea");
            Assert.AreEqual(_actionIp.ToString(), "10.10.10.10");
            Assert.AreEqual(_actionLocation.RegionName,"testregion");
            Assert.AreEqual(_actionLocation.Latitude, 0);
            Assert.AreEqual(_actionLocation.Longitude, 0);
        }

        [TestMethod]
        public void TestIPGeoFencingEngine_ShouldFireDefaultAction()
        {
            var ip2Locationprovider = GetTestIP2LocationProvider();
            var geographicAreaProvider = GetTestGeographicAreasProvider();
            var neverMatchingRules = GetNeverMatchingGeofencingRules();
            var defaultActions = GetTestDefaultActions();

            var engine = new IPGeoFencingEngine(
                                    ip2Locationprovider, 
                                    geographicAreaProvider, 
                                    neverMatchingRules, 
                                    defaultActions);

            engine.Run("10.10.10.10");

            Assert.IsNotNull(_defaultActionIp);
            Assert.AreEqual(_defaultActionIp.ToString(), "10.10.10.10");

            Assert.IsNull(_actionAreas);
            Assert.IsNull(_actionIp);
            Assert.IsNull(_actionLocation);

        }
    }
}