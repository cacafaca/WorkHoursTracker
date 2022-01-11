using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class AddLogViewModel : BaseViewModel
    {
        private string _log;

        public string Log
        {
            get
            {
                return ReadLog();
            }
            set
            {
                _log = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SaveLogCanExecuteFlag));
            }
        }

        private string ReadLog()
        {
            if (_isLoaded)
            {
                return _log;
            }
            else
            {
                try
                {
                    WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel(new Model.Config().WorkHoursCurrentFilePath);
                    whExcel.Read();
                    _isLoaded = true;
                    _originalLog = whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log ?? string.Empty;
                    return _originalLog;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    return ex.Message;
                }
            }
        }

        private bool _isLoaded;
        private string _originalLog = string.Empty;

        public IWindowFactory ConfigWindowFactory { get; set; }

        public AddLogViewModel()
        {
            OpenConfigCommand = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
            SaveLogCommand = new RelayCommand(SaveLogExecute, SaveLogCanExecute);
            CancelLogCommand = new RelayCommand(CancelLogExecute, new Func<object, bool>((obj) => true));
            _isLoaded = false;
        }

        #region Config command
        public ICommand OpenConfigCommand { get; set; }
        private void ConfigExecute(object sender)
        {
            if (ConfigWindowFactory != null)
            {
                ConfigWindowFactory.CreateWindow();
                ConfigWindowFactory.ShowWindow();
            }
        }
        #endregion

        #region Save commnad
        public ICommand SaveLogCommand { get; set; }
        private void SaveLogExecute(object sender)
        {
            // Fire (saving) and forget.
            Task.Run(() =>
            {
                try
                {
                    WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel((new Model.Config()).WorkHoursCurrentFilePath);
                    Trace.WriteLine($"Read '{whExcel.WorkHoursExcelFilePath}'.");
                    whExcel.Read();
                    whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log = Log;
                    Trace.WriteLine($"Log value: '{whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log}'.");
                    Trace.WriteLine($"Write '{whExcel.WorkHoursExcelFilePath}'.");
                    whExcel.Write();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            });

            if (DefaultWindowFactory != null)
            {
                DefaultWindowFactory.CloseWindow();
            }
        }
        private bool SaveLogCanExecute(object obj)
        {
            return _originalLog != (Log ?? string.Empty);
        }
        #endregion

        public bool SaveLogCanExecuteFlag { get { return SaveLogCanExecute(null); } }

        #region Cancel command
        public ICommand CancelLogCommand { get; set; }
        private void CancelLogExecute(object sender)
        {
            if (DefaultWindowFactory != null)
            {
                DefaultWindowFactory.CloseWindow();
            }
        }
        #endregion
    }
}
