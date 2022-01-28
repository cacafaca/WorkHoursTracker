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
        #region Fields
        readonly string _workHoursExcelFilePath;
        private List<Exception> _exceptions;
        #endregion

        #region Constructors
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
        #endregion

        #region Properties
        public List<Exception> Exceptions { get { return _exceptions; } }
        public string WorkHoursExcelFilePath { get { return _workHoursExcelFilePath; } }
        #endregion

        #region Methods
        public void Read()
        {
            ValidateExcelPath();

            Microsoft.Office.Interop.Excel.Application? excelApp = null;
            Microsoft.Office.Interop.Excel.Workbook? workbook = null;
            Microsoft.Office.Interop.Excel.Worksheet? worksheet = null;
            try
            {
                // Copy if do not exists.
                if (!File.Exists(_workHoursExcelFilePath))
                {
                    Trace.WriteLine($"File '{_workHoursExcelFilePath}' do not existis.");
                    File.Copy(GetTemplateFileName(), _workHoursExcelFilePath);
                    Trace.WriteLine($"Template '{GetTemplateFileName()}' copied to '{_workHoursExcelFilePath}'.");
                }

                if (File.Exists(_workHoursExcelFilePath))
                {
                    excelApp = new Microsoft.Office.Interop.Excel.Application();
                    Trace.WriteLine($"Opening Excel with file '{_workHoursExcelFilePath}' ...");
                    workbook = excelApp.Workbooks.Open(_workHoursExcelFilePath);
                    worksheet = workbook.Worksheets[1];    // Expecting data on first sheet, always. First sheet starts from 1, not 0!

                    _employee = new Employee
                    {
                        EmployeeID = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.EmployeeId.Row, WorkHoursExcelMap.EmployeeId.Column]).Value ?? DefaultEmployeeId,
                        FirstAndLastName = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.FirstAndLastName.Row, WorkHoursExcelMap.FirstAndLastName.Column]).Value ?? DefaultFirstAndLastName,
                        Title = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.Title.Row, WorkHoursExcelMap.Title.Column]).Value ?? DefaultTitle,
                        Department = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.Department.Row, WorkHoursExcelMap.Department.Column]).Value ?? DefaultDepartment,
                    };

                    // Correct template values.
                    if (_employee.EmployeeID == DefaultEmployeeId)
                        _employee.EmployeeID = Environment.UserName;
                    // Don't read AD unnecessary.
                    Services.ActiveDirectory ad = null;
                    if (_employee.FirstAndLastName == DefaultFirstAndLastName)
                    {
                        if (ad == null) ad = new Services.ActiveDirectory();
                        _employee.FirstAndLastName = ad.FirstAndLastName;
                    }
                    if (_employee.Title == DefaultTitle)
                    {
                        if (ad == null) ad = new Services.ActiveDirectory();
                        _employee.Title = ad.Title;
                    }
                    if (_employee.Department == DefaultDepartment)
                    {
                        if (ad == null) ad = new Services.ActiveDirectory();
                        _employee.Department = ad.Department;
                    }
                    // Change worksheet name to be as user's first and last name.
                    if (worksheet.Name == DefaultFirstAndLastName)
                        worksheet.Name = _employee.FirstAndLastName;

                    _startDate = GetDateOnly(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.StartDate.Row, WorkHoursExcelMap.StartDate.Column]).Value);
                    // Correct values.
                    bool startDateValid = true;
                    if (StartDate != DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)))
                    {
                        StartDate = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                        startDateValid = false;
                    }

                    object numberOfWorkingDaysPerMonthTemp = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.NumberOfWorkingDays.Row, WorkHoursExcelMap.NumberOfWorkingDays.Column]).Value;
                    if (numberOfWorkingDaysPerMonthTemp.GetType().Name == typeof(double).Name)
                        _numberOfWorkingDaysPerMonth = Convert.ToInt32((double)numberOfWorkingDaysPerMonthTemp);
                    else
                        _numberOfWorkingDaysPerMonth = DateTime.DaysInMonth(_startDate.Year, _startDate.Month);

                    for (int day = 0; day < 31; day++)
                    {
                        WorkHours dailyWorkHours = new WorkHours
                        {
                            Date = startDateValid ? GetDateOnly(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.DateFirstRow.Row + day, WorkHoursExcelMap.DateFirstRow.Column]).Value) : StartDate.AddDays(day),
                            Time1In = GetTimeOnly(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.Time1InFirstRow.Row + day, WorkHoursExcelMap.Time1InFirstRow.Column]).Value),
                            Time1Out = GetTimeOnly(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.TimeOut1FirstRow.Row + day, WorkHoursExcelMap.TimeOut1FirstRow.Column]).Value),
                            Task = GetAsText(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.TaskFirstRow.Row + day, WorkHoursExcelMap.TaskFirstRow.Column]).Value),
                            Log = GetAsText(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.LogFirstRow.Row + day, WorkHoursExcelMap.LogFirstRow.Column]).Value),
                        };
                        _workHours.Add(dailyWorkHours);
                    }
                }
                else
                {
                    throw new ArgumentException($"File '{_workHoursExcelFilePath}' don't exists.");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                SafeCloseExcel(ref excelApp, ref workbook, ref worksheet);
            }
        }
        public void Show()
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
        public void Write()
        {
            if (string.IsNullOrWhiteSpace(_workHoursExcelFilePath))
                throw new ArgumentException("Please provide a valid filename.", nameof(_workHoursExcelFilePath));

            if (IsFileAvailable(_workHoursExcelFilePath))
            {
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
                    ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.StartDate.Row, WorkHoursExcelMap.StartDate.Column]).Value = _startDate.ToDateTime(TimeOnly.MinValue);
                    // Days in month.
                    ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.NumberOfWorkingDays.Row, WorkHoursExcelMap.NumberOfWorkingDays.Column]).Value = _numberOfWorkingDaysPerMonth;

                    if (_workHours != null)
                        for (int day = 0; day < _workHours.Count; day++)
                        {
                            ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.Time1InFirstRow.Row + day, WorkHoursExcelMap.Time1InFirstRow.Column]).Value = _workHours[day].Time1In.ToString();
                            ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.TimeOut1FirstRow.Row + day, WorkHoursExcelMap.TimeOut1FirstRow.Column]).Value = _workHours[day].Time1Out.ToString();
                            // ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.TaskFirstRow.Row + day, WorkHoursExcelMap.TaskFirstRow.Column]).Value = _workHours[day].Task;   // Task is locked.
                            ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[WorkHoursExcelMap.LogFirstRow.Row + day, WorkHoursExcelMap.LogFirstRow.Column]).Value = _workHours[day].Log;
                        }
                    excelApp.DisplayAlerts = false;
                    workbook.Save();
                }
                finally
                {
                    SafeCloseExcel(ref excelApp, ref workbook, ref worksheet);
                }
            }
            else
            {
                throw new Exception($"File '{_workHoursExcelFilePath}' can't be open. Maybe it is open in another app.");
            }
        }
        public string GetTemplateFileName()
        {
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.Settings.Default.WorkHoursTemplateFileName);
        }
        private void SafeCloseExcel(ref Microsoft.Office.Interop.Excel.Application? excelApp, ref Microsoft.Office.Interop.Excel.Workbook? workbook,
            ref Microsoft.Office.Interop.Excel.Worksheet? worksheet)
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
        private bool IsFileAvailable(string filePath)
        {
            try
            {
                Stream s = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                s.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
        private string? GetAsText(object textObj)
        {
            string? text = null;
            if (textObj != null)
            {
                if (textObj.GetType().Name == typeof(string).Name)
                    text = textObj.ToString();
                else if (textObj.GetType().Name == typeof(double).Name)
                {

                    text = textObj.ToString();

                }
                else
                {
                    text = textObj.ToString();
                    _exceptions.Add(new ArgumentException($"Date '{textObj}' is of unknown type ('{textObj.GetType().Name}')."));
                }
            }
            return text;
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
        private void ValidateExcelPath()
        {
            if (string.IsNullOrWhiteSpace(_workHoursExcelFilePath))
                throw new ArgumentException("Please provide a valid filename.", nameof(_workHoursExcelFilePath));
        }
        #endregion
    }
}
