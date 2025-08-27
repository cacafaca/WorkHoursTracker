using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProCode.WorkHoursTracker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.Tests
{
    [TestClass()]
    public class WorkHoursExcelTests
    {
        const string testFileName = "WorkHoursTrackTest_November2021.xlsx";

        [TestMethod()]
        public void Read_November_2021()
        {
            WorkHoursMonthlyExcel workHours = new WorkHoursMonthlyExcel(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), testFileName));
            Assert.IsNotNull(workHours);

            workHours.Read();

            Assert.IsNotNull(workHours.Employee);
            Assert.AreEqual("<EmployeeID>", workHours.Employee.EmployeeID);
            Assert.AreEqual("<FirstName_LastName>", workHours.Employee.FirstAndLastName);
            Assert.AreEqual("<Title>", workHours.Employee.Title);
            Assert.AreEqual("<Department>", workHours.Employee.Department);

            Assert.AreEqual(DateOnly.Parse("1.11.2021"), workHours.StartDate);
            Assert.AreEqual(31, workHours.NumberOfWorkingDaysPerMonth);

            Assert.IsNotNull(workHours.WorkHours);
            Assert.AreNotEqual(0, workHours.WorkHours.Count);

            // Date #1.
            Assert.AreEqual(DateOnly.Parse("1.11.2021"), workHours.WorkHours[0].Date);
            Assert.AreEqual(TimeOnly.Parse("9:00:00"), workHours.WorkHours[0].Time1In);
            Assert.AreEqual(TimeOnly.Parse("17:00:00"), workHours.WorkHours[0].Time1Out);
            Assert.AreEqual("Test task 1", workHours.WorkHours[0].Task);
            Assert.AreEqual("Test log 1", workHours.WorkHours[0].Log);

            // Date #30.
            Assert.AreEqual(DateOnly.Parse("30.11.2021"), workHours.WorkHours[29].Date);
            Assert.AreEqual(TimeOnly.Parse("9:00:00"), workHours.WorkHours[29].Time1In);
            Assert.AreEqual(TimeOnly.Parse("17:00:00"), workHours.WorkHours[29].Time1Out);
            Assert.AreEqual("Test task 30", workHours.WorkHours[29].Task);
            Assert.AreEqual("Test log 30", workHours.WorkHours[29].Log);
        }

        [TestMethod()]
        public void Save_November_2021()
        {
            string expectedTimeIn = "8:00";
            string expectedTimeOut = "16:00";
            
            // Read.
            WorkHoursMonthlyExcel workHoursRead = new WorkHoursMonthlyExcel(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), testFileName));
            Assert.IsNotNull(workHoursRead);
            workHoursRead.Read();

            // Add.
            workHoursRead.WorkHours[1].Time1In = TimeOnly.Parse(expectedTimeIn);
            workHoursRead.WorkHours[1].Time1Out = TimeOnly.Parse(expectedTimeOut);
            workHoursRead.WorkHours[1].Task = "Task 2";
            workHoursRead.WorkHours[1].Log = "Log 2";

            // Save.
            WorkHoursMonthlyExcel workHoursSave = new WorkHoursMonthlyExcel(workHoursRead.Employee, workHoursRead.StartDate, workHoursRead.NumberOfWorkingDaysPerMonth,
                workHoursRead.WorkHours, Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WorkHoursTrackTest_November2021_Save.xlsx"));
            workHoursSave.Write();

            // Read to check added data.
            workHoursRead = new WorkHoursMonthlyExcel(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WorkHoursTrackTest_November2021.xlsx"));
            Assert.IsNotNull(workHoursRead);
            workHoursRead.Read();
            
            Assert.IsNotNull(workHoursRead.WorkHours);
            Assert.AreEqual(TimeOnly.Parse(expectedTimeIn), workHoursSave.WorkHours[1].Time1In);
            Assert.AreEqual(TimeOnly.Parse(expectedTimeOut), workHoursSave.WorkHours[1].Time1Out);
        }
    }
}