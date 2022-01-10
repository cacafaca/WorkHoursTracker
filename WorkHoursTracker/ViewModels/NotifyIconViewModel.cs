using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class NotifyIconViewModel : BaseViewModel
    {
        public ICommand AddLogCommand { get; set; }
        public ICommand ConfigCommand { get; set; }
        public ICommand ExitApplicationCommand { get; set; }
        public ICommand ShowExcelCommand { get; set; }

        public NotifyIconViewModel()
        {
            AddLogCommand = new RelayCommand(AddLogExecute, new Func<object, bool>((obj) => true));
            ConfigCommand = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
            ExitApplicationCommand = new RelayCommand(ExitApplicationExecute, new Func<object, bool>((obj) => true));
            ShowExcelCommand = new RelayCommand(ShowExcelExecute, new Func<object, bool>((obj) => true));
        }

        private void ShowExcelExecute(object obj)
        {
            WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel(new Model.Config().WorkHoursCurrentFilePath);
            whExcel.Show();
        }

        private void ExitApplicationExecute(object obj)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ConfigExecute(object obj)
        {
            if (ConfigWindowFactory != null)
            {
                ConfigWindowFactory.CreateWindow();
                ConfigWindowFactory.ShowWindow();
            }
        }

        private void AddLogExecute(object obj)
        {
            if (WorkHoursLogPopupWindowFactory != null)
            {
                WorkHoursLogPopupWindowFactory.CreateWindow();
                WorkHoursLogPopupWindowFactory.ShowWindow();
            }
        }

        public IWindowFactory WorkHoursLogPopupWindowFactory { get; set; }
        public IWindowFactory ConfigWindowFactory { get; set; }
    }
}
