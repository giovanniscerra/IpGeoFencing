using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.DataModels;

namespace IPGeoFencing.UnitTests.GeographicAreas
{
    [TestClass]
    public class BoundingBoxAreaModelTest: BaseGeographicAreaTest
    {

        [TestMethod]
        public void TestCircle()
        {
            var triangleCoordinates = GetTestTriangleCoordinates(10);

            var boundingBox = new BoundingBoxAreaModel("boundingbox", triangleCoordinates);

            //Test point outside bounding box
            Assert.IsFalse(boundingBox.Contains(20, 20));

            //Test point outside triangle but inside bounding box
            Assert.IsTrue(boundingBox.Contains(10, 10));

            //Test point inside triangle
            Assert.IsTrue(boundingBox.Contains(0, 0));
        }
    }
}