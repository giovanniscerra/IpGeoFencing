using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.DataModels;

namespace IPGeoFencing.UnitTests.GeographicAreas
{
    [TestClass]
    public class MultiPolygonAreaModelTest: BaseGeographicAreaTest
    {
        [TestMethod]
        public void TestMultiPoligons()
        {
            var poly1 = new PolygonAreaModel("poly1", new PerimeterModel[] { GetTestSquarePermeter(10, -50) });
            var poly2 = new PolygonAreaModel("poly2", new PerimeterModel[] { GetTestSquarePermeter(10, 50) });

            var poly = new MultiPolygonAreaModel("multipoly", new PolygonAreaModel[] { poly1, poly2 });

            //Test point outside polygons
            Assert.IsFalse(poly.Contains(0, 0));

            //Test point outside polygons
            Assert.IsFalse(poly.Contains(80, -80));

            //Test point in poly1
            Assert.IsTrue(poly.Contains(-59, -59));

            //Test point in poly2
            Assert.IsTrue(poly.Contains(40, 40));
        }
    }
}