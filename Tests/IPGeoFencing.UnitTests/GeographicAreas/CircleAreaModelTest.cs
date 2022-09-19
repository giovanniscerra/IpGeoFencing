using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.DataModels;

namespace IPGeoFencing.UnitTests.GeographicAreas
{
    [TestClass]
    public class CircleAreaModelTest: BaseGeographicAreaTest
    {

        [TestMethod]
        public void TestCircle()
        {
            var circle = GetTestCircleArea(16);

            //Test point outside circle
            Assert.IsFalse(circle.Contains(50, 50));

            //Test center
            Assert.IsTrue(circle.Contains(0, 0));

            //Test point inside circle
            Assert.IsTrue(circle.Contains(0.0001, -0.0001));
        }
    }
}