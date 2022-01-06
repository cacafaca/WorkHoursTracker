using System;
using System.Collections.Generic;
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
            get { return _log; }
            set
            {
                _log = value;
                OnPropertyChanged();
            }
        }

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

        }
        #endregion
    }
}
