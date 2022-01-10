using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProCode.WorkHoursTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.Services.Tests
{
    [TestClass()]
    public class ActiveDirectoryTests
    {
        [TestMethod()]
        public void ReadTest()
        {
            ActiveDirectory ad = new ActiveDirectory();
            ad.Read();
            Assert.IsNotNull(ad.FirstAndLastName);
            Assert.IsNotNull(ad.Title);
            Assert.IsNotNull(ad.Department);

            Assert.AreNotEqual(string.Empty, ad.FirstAndLastName);
            Assert.AreNotEqual(string.Empty, ad.Title);
            Assert.AreNotEqual(string.Empty, ad.Department);
        }
    }
}