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
        #region Constructors
        public NotifyIconViewModel()
        {
            AddLogCommand = new RelayCommand(AddLogExecute, new Func<object, bool>((obj) => true));
            ConfigCommand = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
            ExitApplicationCommand = new RelayCommand(ExitApplicationExecute, new Func<object, bool>((obj) => true));
            ShowExcelCommand = new RelayCommand(ShowExcelExecute, new Func<object, bool>((obj) => true));
        }
        #endregion

        #region Properties
        public ICommand AddLogCommand { get; set; }
        public ICommand ConfigCommand { get; set; }
        public ICommand ExitApplicationCommand { get; set; }
        public ICommand ShowExcelCommand { get; set; }

        public IWindowFactory AddLogWindowFactory { get; set; }
        public IWindowFactory ConfigWindowFactory { get; set; }

        #endregion

        #region Methods
        private void AddLogExecute(object obj)
        {
            if (AddLogWindowFactory != null)
            {
                if (!AddLogWindowFactory.IsCreated())
                    AddLogWindowFactory.CreateWindow(obj);
                if (!AddLogWindowFactory.IsVisible())
                    AddLogWindowFactory.ShowWindow();
            }
        }

        private void ShowExcelExecute(object obj)
        {
            WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel(Model.Config.WorkHoursCurrentFilePath);
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
                if (!ConfigWindowFactory.IsCreated())
                    ConfigWindowFactory.CreateWindow();
                if (!ConfigWindowFactory.IsVisible())
                    ConfigWindowFactory.ShowWindow();
            }
        }
        #endregion
    }
}
