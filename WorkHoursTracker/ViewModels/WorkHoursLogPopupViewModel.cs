using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class WorkHoursLogPopupViewModel : BaseViewModel
    {
        private string _log;

        public string Log
        {
            get
            {
                return GetLog();
            }
            set
            {
                _log = value;
                OnPropertyChanged();
            }
        }

        private string GetLog()
        {
            if(_isLoaded)
            {
                return _log;
            }
            else
            {
                WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel(new Model.Config().WorkHoursCurrentFilePath);
                return whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log;
            }
        }

        private bool _isLoaded;

        public IWindowFactory ConfigWindowFactory { get; set; }

        public WorkHoursLogPopupViewModel()
        {
            OpenConfigCommnad = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
            AcceptCommnad = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
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

        #region Accept commnad
        public ICommand AcceptCommnad { get; set; }
        private void AcceptExecute(object sender)
        {
            WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel((new Model.Config()).WorkHoursDirectory);
            whExcel.WorkHours.Where(wh=>wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log = Log;
            whExcel.Write();
        }
        #endregion
    }
}
