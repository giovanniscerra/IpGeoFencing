using IPGeoFencing.IP2Location.DataModels;
using IPGeoFencing.IP2Location.Providers;

namespace IPGeoFencing.UnitTests.IP2Location
{
    [TestClass]
    public class IP2LocationInMemoryProviderTest
    {

        private IP2LocationInMemoryProvider GetTestProvider()
        {
            var locations = new List<IPLocationModel>();

            locations.Add(
                    IPLocationModel.Create(
                    ipFrom: 0,
                    ipTo: 2,
                    regionName: "Region1")
            );

            locations.Add(
                    IPLocationModel.Create(
                    ipFrom: 3,
                    ipTo: 5,
                    regionName: "Region2")
            );

            return new IP2LocationInMemoryProvider(locations);
    }


        [TestMethod]
        public void TestIP2LocationInMemoryProvider()
        {
            var ip2LocationProvider = GetTestProvider();

            //ip in region 1
            Assert.AreEqual(ip2LocationProvider.GetLocationFromIP(1)?.RegionName, "Region1");

            //ip in region 1
            Assert.AreEqual(ip2LocationProvider.GetLocationFromIP(2)?.RegionName, "Region1");

            //ip in region 2
            Assert.AreEqual(ip2LocationProvider.GetLocationFromIP(3)?.RegionName, "Region2");

            //ip not found in any region
            Assert.ThrowsException<InvalidDataException>(()=> { ip2LocationProvider.GetLocationFromIP(100); });

        }
    }
}