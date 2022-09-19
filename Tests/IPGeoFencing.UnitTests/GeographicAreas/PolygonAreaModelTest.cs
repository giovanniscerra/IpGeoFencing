using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.DataModels;

namespace IPGeoFencing.UnitTests.GeographicAreas
{
    [TestClass]
    public class PolygonAreaModelTest: BaseGeographicAreaTest
    {
        [TestMethod]
        public void TestSquarePoligonWithHole()
        {
            var poly = new PolygonAreaModel("poly", new PerimeterModel[] { GetTestSquarePermeter(10), GetTestSquarePermeter(3) });

            //Test point in hole
            Assert.IsFalse(poly.Contains(0, 0));

            //Test point in hole edge
            Assert.IsFalse(poly.Contains(0, 3));

            //Test point in hole vertex
            Assert.IsFalse(poly.Contains(-3, -3));

            //Test point in poligon
            Assert.IsTrue(poly.Contains(-4, -4));

            //Test point on poligon edge
            Assert.IsTrue(poly.Contains(0, 5));

            //Test point on poligon vertex
            Assert.IsTrue(poly.Contains(5, 5));
        }
    }
}