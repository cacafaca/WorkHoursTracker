using System;
using System.Collections.Generic;

namespace ProCode.WorkHoursTracker
{
    public class WorkHoursMonthlyModel
    {
        const string defaultEmployeeId = "<EmployeeID>";
        const string defaultFirstAndLastName = "<FirstName_LastName>";
        const string defaultTitle = "<Title>";
        const string defaultDepartment = "<Department>";

        protected Employee? _employee;
        public Employee? Employee { get { return _employee; } }

        protected DateOnly _startDate;
        public DateOnly StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        protected int _numberOfWorkingDaysPerMonth;
        public int NumberOfWorkingDaysPerMonth
        {
            get { return _numberOfWorkingDaysPerMonth; }
            set { _numberOfWorkingDaysPerMonth = value; }
        }

        protected List<WorkHours> _workHours;
        public List<WorkHours> WorkHours { get { return _workHours; } }

        public WorkHoursMonthlyModel(Employee employee, DateOnly startDate, int numberOfWorkingDaysPerMonth, List<WorkHours> workHours)
        {
            _employee = employee;
            _startDate = startDate;
            _numberOfWorkingDaysPerMonth = numberOfWorkingDaysPerMonth;
            _workHours = workHours;
        }
        public WorkHoursMonthlyModel()
        {
            _employee = new Employee
            {
                EmployeeID = Environment.UserDomainName + "\\" + Environment.UserName,
                Department = "IT",
                Title = String.Empty,
                FirstAndLastName = String.Empty
            };
            _startDate = DateOnly.FromDateTime(DateTime.Now);
            _numberOfWorkingDaysPerMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            _workHours = new List<WorkHours>();
            for (int day = 1; day <= 31; day++)
            {
                _workHours.Add(new WorkHours
                {
                    Date = _startDate.AddDays(day - 1),
                });
            }
        }

        public WorkHoursMonthlyModel GetDefaultValues()
        {
            var daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var wh = new List<WorkHours>();
            for (var i = 0; i < daysInMonth; i++)
                wh.Add(new WorkHours
                {
                    Date = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i+1))                    
                });

            return new WorkHoursMonthlyModel(
                employee: new Employee
                {
                    EmployeeID = defaultEmployeeId,
                    Department = defaultDepartment,
                    Title = defaultTitle,
                    FirstAndLastName = defaultFirstAndLastName
                },
                startDate: DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)),
                numberOfWorkingDaysPerMonth: daysInMonth,
                workHours: wh);            
        }
    }
}
