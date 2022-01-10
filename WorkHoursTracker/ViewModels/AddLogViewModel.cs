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
                WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel(new Model.Config().WorkHoursCurrentFilePath);
                whExcel.Read();
                _isLoaded = true;
                return whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log;
            }
        }

        private bool _isLoaded;

        public IWindowFactory ConfigWindowFactory { get; set; }

        public AddLogViewModel()
        {
            OpenConfigCommnad = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
            SaveCommnad = new RelayCommand(SaveExecute, new Func<object, bool>((obj) => true));
            _isLoaded = false;
        }

        #region Config command
        public ICommand OpenConfigCommnad { get; set; }
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
        public ICommand SaveCommnad { get; set; }
        private void SaveExecute(object sender)
        {
            // Fire (saving) and forget.
            Task.Run(() =>
            {
                WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel((new Model.Config()).WorkHoursCurrentFilePath);
                whExcel.Read();
                whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log = Log;
                whExcel.Write();
            });

            if (DefaultWindowFactory != null)
            {
                DefaultWindowFactory.CloseWindow();
            }
        }
        #endregion
    }
}
