using System;
using System.Windows.Input;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class NotifyIconViewModel : BaseViewModel
    {
        #region Fields
        string _showCurrentWorkHoursDisplayName;
        string _showPreviousWorkHoursDisplayName;
        #endregion

        #region Constructors
        public NotifyIconViewModel()
        {
            AddLogCommand = new RelayCommand(AddLogExecute, new Func<object, bool>((obj) => true));
            ConfigCommand = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
            ExitApplicationCommand = new RelayCommand(ExitApplicationExecute, new Func<object, bool>((obj) => true));
            ShowCurrentWorkHoursCommand = new RelayCommand(ShowCurrentWorkHoursExecute, new Func<object, bool>((obj) => true));
            ShowPreviousWorkHoursCommand = new RelayCommand(ShowPreviousWorkHoursExecute, new Func<object, bool>((obj) => true));
            ShowCurrentWorkHoursDisplayName = $"Show Work Hours ({DateTime.Today:MMM})";
            ShowPreviousWorkHoursDisplayName = $"Show Work Hours ({DateTime.Today.AddMonths(-1):MMM})";
        }
        #endregion

        #region Properties
        public ICommand AddLogCommand { get; set; }
        public ICommand ConfigCommand { get; set; }
        public ICommand ExitApplicationCommand { get; set; }
        public ICommand ShowCurrentWorkHoursCommand { get; set; }
        public ICommand ShowPreviousWorkHoursCommand { get; set; }

        public IWindowFactory AddLogWindowFactory { get; set; }
        public IWindowFactory ConfigWindowFactory { get; set; }

        public string ShowCurrentWorkHoursDisplayName
        {
            get
            {
                return _showCurrentWorkHoursDisplayName;
            }
            set
            {
                _showCurrentWorkHoursDisplayName = value;
                OnPropertyChanged();
            }
        }
        public string ShowPreviousWorkHoursDisplayName
        {
            get
            {
                return _showPreviousWorkHoursDisplayName;
            }
            set
            {
                _showPreviousWorkHoursDisplayName = value;
                OnPropertyChanged();
            }
        }
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
        private void ShowCurrentWorkHoursExecute(object obj)
        {
            WorkHoursMonthlyExcel whExcel = new(Model.Config.WorkHoursCurrentFilePath);
            whExcel.Show();
        }
        private void ShowPreviousWorkHoursExecute(object obj)
        {
            WorkHoursMonthlyExcel whExcel = new(Model.Config.WorkHoursPreviousFilePath);
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
