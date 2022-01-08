using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker
{
    public class WorkHoursMonthlyExcel : WorkHoursMonthlyModel
    {
        readonly string _workHoursExcelFilePath;

        public WorkHoursMonthlyExcel(string workHoursExcelFilePath)
        {
            _workHoursExcelFilePath = workHoursExcelFilePath;
            _workHours = new List<WorkHours>();
            _exceptions = new List<Exception>();
        }
        public WorkHoursMonthlyExcel(Employee employee, DateOnly startDate, int numberOfWorkingDays, List<WorkHours> workHours, string workHoursExcelPath) : this(workHoursExcelPath)
        {
            _employee = employee;
            _startDate = startDate;
            _numberOfWorkingDaysPerMonth = numberOfWorkingDays;
            _workHours = new List<WorkHours>(workHours);
        }

        private List<Exception> _exceptions;
        public List<Exception> Exceptions { get { return _exceptions; } }

        private void ValidateExcelPath()
        {
            if (string.IsNullOrWhiteSpace(_workHoursExcelFilePath))
                throw new ArgumentException("Please provide a valid filename.", nameof(_workHoursExcelFilePath));
        }

        public void Read()
        {
            ValidateExcelPath();

            Microsoft.Office.Interop.Excel.Application? excelApp = null;
            Microsoft.Office.Interop.Excel.Workbook? workbook = null;
            Microsoft.Office.Interop.Excel.Worksheet? worksheet = null;
            try
            {
                if (File.Exists(_workHoursExcelFilePath))
                {
                    excelApp = new Microsoft.Office.Interop.Excel.Application();
                    workbook = excelApp.Workbooks.Open(_workHoursExcelFilePath);
                    worksheet = workbook.Worksheets[1];    // Expecting data on first sheet, always. First sheet starts from 1, not 0!

                    _employee = new Employee
                    {
                        EmployeeID = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.EmployeeId.Row, WorkHoursExcelMap.EmployeeId.Column]).Value,
                        FirstAndLastName = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.FirstAndLastName.Row, WorkHoursExcelMap.FirstAndLastName.Column]).Value,
                        Title = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.Title.Row, WorkHoursExcelMap.Title.Column]).Value,
                        Department = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.Department.Row, WorkHoursExcelMap.Department.Column]).Value,
                    };

                    _startDate = GetDateOnly(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.StartDate.Row, WorkHoursExcelMap.StartDate.Column]).Value);

                    object numberOfWorkingDaysPerMonthTemp = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.NumberOfWorkingDays.Row, WorkHoursExcelMap.NumberOfWorkingDays.Column]).Value;
                    if (numberOfWorkingDaysPerMonthTemp.GetType().Name == typeof(double).Name)
                        _numberOfWorkingDaysPerMonth = Convert.ToInt32((double)numberOfWorkingDaysPerMonthTemp);
                    else
                        _numberOfWorkingDaysPerMonth = DateTime.DaysInMonth(_startDate.Year, _startDate.Month);

                    for (int day = 0; day < 31; day++)
                    {
                        WorkHours dailyWorkHours = new WorkHours
                        {
                            Date = GetDateOnly(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.DateFirstRow.Row + day, WorkHoursExcelMap.DateFirstRow.Column]).Value),
                            Time1In = GetTimeOnly(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.Time1InFirstRow.Row + day, WorkHoursExcelMap.Time1InFirstRow.Column]).Value),
                            Time1Out = GetTimeOnly(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.TimeOut1FirstRow.Row + day, WorkHoursExcelMap.TimeOut1FirstRow.Column]).Value),
                            Task = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.TaskFirstRow.Row + day, WorkHoursExcelMap.TaskFirstRow.Column]).Value,
                            Log = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.LogFirstRow.Row + day, WorkHoursExcelMap.LogFirstRow.Column]).Value,
                        };
                        _workHours.Add(dailyWorkHours);
                    }
                }
                else
                {
                    throw new ArgumentException($"File '{_workHoursExcelFilePath}' don't exists.");
                }
            }
            finally
            {
                if (worksheet != null)
                {
                    while (Marshal.ReleaseComObject(worksheet) != 0) { };
                    worksheet = null;
                }
                if (workbook != null)
                {
                    workbook.Close(false);
                    while (Marshal.ReleaseComObject(workbook) != 0) { };
                    workbook = null;
                }
                if (excelApp != null)
                {
                    if (excelApp.Workbooks != null)
                    {
                        excelApp.Workbooks.Close();
                        while (Marshal.ReleaseComObject(excelApp.Workbooks) != 0) { };
                    }

                    excelApp.Application.Quit();
                    excelApp.Quit();
                    while (Marshal.ReleaseComObject(excelApp) != 0) { };
                    excelApp = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public void Open()
        {
            ValidateExcelPath();

            if (!File.Exists(_workHoursExcelFilePath))
            {
                File.Copy(GetTemplateFileName(), _workHoursExcelFilePath);
            }

            Microsoft.Office.Interop.Excel.Application? excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook? workbook = excelApp.Workbooks.Open(_workHoursExcelFilePath);
            excelApp.Visible = true;
        }

        private DateOnly? GetDateOnly(object dateObj)
        {
            DateOnly? date = null;
            if (dateObj != null)
            {
                if (dateObj.GetType().Name == typeof(DateTime).Name)
                    date = DateOnly.FromDateTime((DateTime)dateObj);
                else if (dateObj.GetType().Name == typeof(string).Name)
                {
                    if (DateOnly.TryParse((string)dateObj, out DateOnly dateTmp))
                        date = dateTmp;
                    else
                    {
                        date = DateOnly.MinValue;
                        _exceptions.Add(new ArgumentException($"Can't parse date '{dateObj}'."));
                    }
                }
                else
                {
                    date = DateOnly.MinValue;
                    _exceptions.Add(new ArgumentException($"Date '{dateObj}' is of unknown type ('{dateObj.GetType().Name}')."));
                }
            }
            return date;
        }

        private TimeOnly? GetTimeOnly(object timeObj)
        {
            TimeOnly? time = null;
            if (timeObj != null)
            {
                if (timeObj.GetType().Name == typeof(double).Name)
                    time = TimeOnly.FromDateTime((DateTime.FromOADate((double)timeObj)));
                else if (timeObj.GetType().Name == typeof(string).Name)
                {
                    if (TimeOnly.TryParse((string)timeObj, out TimeOnly timeTmp))
                        time = timeTmp;
                    else
                    {
                        time = TimeOnly.MinValue;
                        _exceptions.Add(new ArgumentException($"Can't parse time '{timeObj}'."));
                    }
                }
                else
                {
                    time = TimeOnly.MinValue;
                    _exceptions.Add(new ArgumentException($"Time '{timeObj}' is of unknown type ('{timeObj.GetType().Name}')."));
                }
            }
            return time;
        }

        private void CreateNewWorkHoursFile()
        {
            File.Copy(GetTemplateFileName(), _workHoursExcelFilePath);

        }

        public void Write()
        {
            if (string.IsNullOrWhiteSpace(_workHoursExcelFilePath))
                throw new ArgumentException("Please provide a valid filename.", nameof(_workHoursExcelFilePath));

            Microsoft.Office.Interop.Excel.Application? excelApp = null;
            Microsoft.Office.Interop.Excel.Workbook? workbook = null;
            Microsoft.Office.Interop.Excel.Worksheet? worksheet = null;
            try
            {
                // Copy copy template if requested file do not exists, because it's going to open a brand new file.
                if (!File.Exists(_workHoursExcelFilePath))
                {
                    File.Copy(GetTemplateFileName(), _workHoursExcelFilePath);
                }

                excelApp = new Microsoft.Office.Interop.Excel.Application();
                workbook = excelApp.Workbooks.Open(_workHoursExcelFilePath);
                worksheet = workbook.Worksheets[1];    // Expecting data on first sheet, always. First sheet starts from 1, not 0!

                if (_employee != null)
                {
                    ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.EmployeeId.Row, WorkHoursExcelMap.EmployeeId.Column]).Value = _employee.EmployeeID;
                    ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.FirstAndLastName.Row, WorkHoursExcelMap.FirstAndLastName.Column]).Value = _employee.FirstAndLastName;
                    ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.Title.Row, WorkHoursExcelMap.Title.Column]).Value = _employee.Title;
                    ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.Department.Row, WorkHoursExcelMap.Department.Column]).Value = _employee.Department;
                }
                    
                // Start date.
                ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.StartDate.Row, WorkHoursExcelMap.StartDate.Column]).Value = _startDate.ToDateTime(new TimeOnly());
                // Days in month.
                ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.NumberOfWorkingDays.Row, WorkHoursExcelMap.NumberOfWorkingDays.Column]).Value = _numberOfWorkingDaysPerMonth;

                if (_workHours != null)
                    for (int day = 0; day < _workHours.Count; day++)
                    {
                        ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.DateFirstRow.Row + day, WorkHoursExcelMap.DateFirstRow.Column]).Value = _workHours[day].Date.ToDateTime(new TimeOnly());
                        ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.Time1InFirstRow.Row + day - 1, WorkHoursExcelMap.Time1InFirstRow.Column]).Value = _workHours[day].Time1In.ToString();
                        ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.TimeOut1FirstRow.Row + day - 1, WorkHoursExcelMap.TimeOut1FirstRow.Column]).Value = _workHours[day].Time1Out.ToString();
                        ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.TaskFirstRow.Row + day - 1, WorkHoursExcelMap.TaskFirstRow.Column]).Value = _workHours[day].Task;
                        ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.LogFirstRow.Row + day - 1, WorkHoursExcelMap.LogFirstRow.Column]).Value = _workHours[day].Log;
                    }
                workbook.Save();
            }
            finally
            {
                if (worksheet != null)
                {
                    while (Marshal.ReleaseComObject(worksheet) != 0) { };
                    worksheet = null;
                }
                if (workbook != null)
                {
                    workbook.Close(false);
                    while (Marshal.ReleaseComObject(workbook) != 0) { };
                    workbook = null;
                }
                if (excelApp != null)
                {
                    if (excelApp.Workbooks != null)
                    {
                        excelApp.Workbooks.Close();
                        while (Marshal.ReleaseComObject(excelApp.Workbooks) != 0) { };
                    }

                    excelApp.Application.Quit();
                    excelApp.Quit();
                    while (Marshal.ReleaseComObject(excelApp) != 0) { };
                    excelApp = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public string GetTemplateFileName()
        {
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.Settings.Default.WorkHoursTemplateFileName);
        }
    }
}
