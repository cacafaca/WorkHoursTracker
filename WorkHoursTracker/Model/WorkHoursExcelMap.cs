using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.Model
{
    public static class WorkHoursExcelMap
    {
        // Header data.

        static ExcelCellCoordinates _employeeId = new ExcelCellCoordinates("C6");
        static public ExcelCellCoordinates EmployeeId { get { return _employeeId; } }

        static ExcelCellCoordinates _firstAndLastName = new ExcelCellCoordinates("F6");
        static public ExcelCellCoordinates FirstAndLastName { get { return _firstAndLastName; } }

        static ExcelCellCoordinates _title = new ExcelCellCoordinates("C8");
        static public ExcelCellCoordinates Title { get { return _title; } }

        static ExcelCellCoordinates _department = new ExcelCellCoordinates("F8");
        static public ExcelCellCoordinates Department { get { return _department; } }

        static ExcelCellCoordinates _startDate = new ExcelCellCoordinates("C10");
        static public ExcelCellCoordinates StartDate { get { return _startDate; } }

        static ExcelCellCoordinates _numberOfWorkingDays = new ExcelCellCoordinates("I10");
        static public ExcelCellCoordinates NumberOfWorkingDays { get { return _numberOfWorkingDays; } }


        // Work days table.

        static ExcelCellCoordinates _dateFirstRow = new ExcelCellCoordinates("B14");
        static public ExcelCellCoordinates DateFirstRow { get { return _dateFirstRow; } }

        static ExcelCellCoordinates _dayOfWeekFirstRow = new ExcelCellCoordinates("C14");
        static public ExcelCellCoordinates DayOfWeekFirstRow { get { return _dayOfWeekFirstRow; } }

        static ExcelCellCoordinates _timeIn1FirstRow = new ExcelCellCoordinates("D14");
        static public ExcelCellCoordinates Time1InFirstRow { get { return _timeIn1FirstRow; } }

        static ExcelCellCoordinates _timeOut1FirstRow = new ExcelCellCoordinates("E14");
        static public ExcelCellCoordinates TimeOut1FirstRow { get { return _timeOut1FirstRow; } }

        static ExcelCellCoordinates _taskFirstRow = new ExcelCellCoordinates("J14");
        static public ExcelCellCoordinates TaskFirstRow { get { return _taskFirstRow; } }

        static ExcelCellCoordinates _logFirstRow = new ExcelCellCoordinates("K14");
        static public ExcelCellCoordinates LogFirstRow { get { return _logFirstRow; } }
    }
}
