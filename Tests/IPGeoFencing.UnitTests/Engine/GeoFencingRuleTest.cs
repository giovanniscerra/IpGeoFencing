using IPGeoFencing.Engine;

namespace IPGeoFencing.UnitTests.Engine
{
    [TestClass]
    public class GeoFencingRuleTest
    {
        private bool _actionExecuted;

        private GeoFencingRule GetTestRule(bool isMatch)
        {
            _actionExecuted = false;

            return new GeoFencingRule("test",
                predicate: (areas, ip, location) => { return isMatch; },
                action: (areas, ip, location) => { _actionExecuted = true; });
        }

        [TestMethod]
        public void TestGeoFencingRule()
        {
            GeoFencingRule testRule;

            //always matching rule
            testRule = GetTestRule(true);
            Assert.IsTrue(testRule.IsMatch(null, null, null));
            testRule.Execute(null, null, null);
            Assert.IsTrue(_actionExecuted);

            //never matching rule
            testRule = GetTestRule(false);
            Assert.IsFalse(testRule.IsMatch(null, null, null));
            Assert.IsFalse(_actionExecuted);
        }
    }
}