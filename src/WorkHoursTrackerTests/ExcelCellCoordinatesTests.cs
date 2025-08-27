using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProCode.WorkHoursTracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.Tests
{
    [TestClass()]
    public class ExcelCellCoordinatesTests
    {
        [TestMethod()]
        public void A_To_1()
        {
            string columnStr = "A";
            int expectedColumnIndex = 1;
            ExcelCellCoordinates coord = new ExcelCellCoordinates(columnStr, 1);
            Assert.AreEqual(expectedColumnIndex, coord.Column);
        }
        [TestMethod()]
        public void B_To_2()
        {
            string columnStr = "B";
            int expectedColumnIndex = 2;
            ExcelCellCoordinates coord = new ExcelCellCoordinates(columnStr, 1);
            Assert.AreEqual(expectedColumnIndex, coord.Column);
        }
        [TestMethod()]
        public void Z_To_26()
        {
            string columnStr = "Z";
            int expectedColumnIndex = 26;
            ExcelCellCoordinates coord = new ExcelCellCoordinates(columnStr, 1);
            Assert.AreEqual(expectedColumnIndex, coord.Column);
        }
        [TestMethod()]
        public void AA_To_27()
        {
            string columnStr = "AA";
            int expectedColumnIndex = 27;
            ExcelCellCoordinates coord = new ExcelCellCoordinates(columnStr, 1);
            Assert.AreEqual(expectedColumnIndex, coord.Column);
        }
        [TestMethod()]
        public void AB_To_28()
        {
            string columnStr = "AB";
            int expectedColumnIndex = 28;
            ExcelCellCoordinates coord = new ExcelCellCoordinates(columnStr, 1);
            Assert.AreEqual(expectedColumnIndex, coord.Column);
        }
        [TestMethod()]
        public void AZ_To_52()
        {
            string columnStr = "AZ";
            int expectedColumnIndex = 52;
            ExcelCellCoordinates coord = new ExcelCellCoordinates(columnStr, 1);
            Assert.AreEqual(expectedColumnIndex, coord.Column);
        }

        [TestMethod()]
        public void A1_To_1_1()
        {
            string cell = "A1";
            int expectedColumnIndex = 1;
            int expectedRow = 1;
            ExcelCellCoordinates coord = new ExcelCellCoordinates(cell);
            Assert.AreEqual(expectedColumnIndex, coord.Column);
            Assert.AreEqual(expectedRow, coord.Row);
        }
        [TestMethod()]
        public void AZ1_To_52_1()
        {
            string cell = "AZ1";
            int expectedColumnIndex = 52;
            int expectedRow = 1;
            ExcelCellCoordinates coord = new ExcelCellCoordinates(cell);
            Assert.AreEqual(expectedColumnIndex, coord.Column);
            Assert.AreEqual(expectedRow, coord.Row);
        }
        [TestMethod()]
        public void AZ11_To_52_11()
        {
            string cell = "AZ11";
            int expectedColumnIndex = 52;
            int expectedRow = 11;
            ExcelCellCoordinates coord = new ExcelCellCoordinates(cell);
            Assert.AreEqual(expectedColumnIndex, coord.Column);
            Assert.AreEqual(expectedRow, coord.Row);
        }
    }
}