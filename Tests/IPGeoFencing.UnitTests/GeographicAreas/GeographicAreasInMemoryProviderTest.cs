using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.Abstractions;
using IPGeoFencing.GeographicAreas.DataModels;
using IPGeoFencing.GeographicAreas.Providers;

namespace IPGeoFencing.UnitTests.GeographicAreas
{
    [TestClass]
    public class GeographicAreasInMemoryProviderTest: BaseGeographicAreaTest
    {

        private GeographicAreasInMemoryProvider GetTestGeographicAreasInMemoryProvider()
        {
            var geographicAreas = new List<GeographicAreaModel>();

            geographicAreas.Add(GetTestCircleArea(5));

            var poly1 = new PolygonAreaModel("poly1", new PerimeterModel[] { GetTestSquarePermeter(5, -50) });
            geographicAreas.Add(poly1);

            var poly2 = new PolygonAreaModel("poly2", new PerimeterModel[] { GetTestSquarePermeter(5, -40) });
            geographicAreas.Add(poly2);

            return new GeographicAreasInMemoryProvider(geographicAreas);
        }

        [TestMethod]
        public void TestGeographicAreasInMemoryProvider()
        {
            var geographicAreasProvider = GetTestGeographicAreasInMemoryProvider();

            IEnumerable<GeographicAreaModel> matchingAreas;

            //point outside all areas
            matchingAreas = geographicAreasProvider.GetAreasContaining(new GeoCoordinate(-20, -20));
            Assert.IsFalse(matchingAreas.Any(m => m.Name == "circle"));
            Assert.IsFalse(matchingAreas.Any(m => m.Name == "poly1"));
            Assert.IsFalse(matchingAreas.Any(m => m.Name == "poly2"));

            //center of circle
            matchingAreas = geographicAreasProvider.GetAreasContaining(new GeoCoordinate(0, 0));
            Assert.IsTrue(matchingAreas.Any(m => m.Name == "circle"));
            Assert.IsFalse(matchingAreas.Any(m => m.Name == "poly1"));
            Assert.IsFalse(matchingAreas.Any(m => m.Name == "poly2"));

            //point only present in poly1
            matchingAreas = geographicAreasProvider.GetAreasContaining(new GeoCoordinate(-55, -55));
            Assert.IsFalse(matchingAreas.Any(m => m.Name == "circle"));
            Assert.IsTrue(matchingAreas.Any(m => m.Name == "poly1"));
            Assert.IsFalse(matchingAreas.Any(m => m.Name == "poly2"));

            //point only present in poly2 vertex
            matchingAreas = geographicAreasProvider.GetAreasContaining(new GeoCoordinate(-35, -35));
            Assert.IsFalse(matchingAreas.Any(m => m.Name == "circle"));
            Assert.IsFalse(matchingAreas.Any(m => m.Name == "poly1"));
            Assert.IsTrue(matchingAreas.Any(m => m.Name == "poly2"));

        }
    }
}