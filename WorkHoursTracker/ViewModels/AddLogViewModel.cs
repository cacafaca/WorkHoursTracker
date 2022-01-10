using System;
using System.Collections.Generic;
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
                    return whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    return ex.Message;
                }
            }
        }

        private bool _isLoaded;

        public IWindowFactory ConfigWindowFactory { get; set; }

        public AddLogViewModel()
        {
            OpenConfigCommand = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
            SaveCommand = new RelayCommand(SaveExecute, new Func<object, bool>((obj) => true));
            CancelCommand = new RelayCommand(CancelExecute, new Func<object, bool>((obj) => true));
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
        public ICommand SaveCommand { get; set; }
        private void SaveExecute(object sender)
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
        #endregion

        #region Cancel command
        public ICommand CancelCommand { get; set; }
        private void CancelExecute(object sender)
        {
            if (DefaultWindowFactory != null)
            {
                DefaultWindowFactory.CloseWindow();
            }
        }
        #endregion
    }
}
