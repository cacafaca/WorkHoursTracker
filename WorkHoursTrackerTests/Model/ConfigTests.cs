using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProCode.WorkHoursTracker.Model.Tests
{
    [TestClass()]
    public class ConfigTests
    {
        [TestMethod()]
        public void GetRegistryPropertiesTest()
        {
            var regProp = Config.GetRegistryProperties();
            Assert.IsNotNull(regProp);
            Assert.AreNotEqual(0, regProp.Count);
        }
    }
}