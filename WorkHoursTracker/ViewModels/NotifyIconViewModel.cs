/*
 * Fuck! I need to move whole business logic to the Model. I put everything in ViewModel. :((
 */

using System;
using System.Windows.Input;
using ProCode.WorkHoursTracker.Views;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class NotifyIconViewModel : BaseViewModel
    {
        #region Fields
        string _showCurrentWorkHoursDisplayName;
        string _showPreviousWorkHoursDisplayName;
        IViewService _viewService;
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
        public ICommand AddLogCommand { get; set; }                     // Command to show entry window as text/table input.
        public ICommand ConfigCommand { get; set; }
        public ICommand ExitApplicationCommand { get; set; }
        public ICommand ShowCurrentWorkHoursCommand { get; set; }
        public ICommand ShowPreviousWorkHoursCommand { get; set; }

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
        public void SetViewService(Views.IViewService viewService)
        {
            _viewService = viewService;
        }

        public IViewService GetViewService()
        {
            return _viewService;
        }

        private void AddLogExecute(object obj)
        {
            switch (Model.Config.AddLogWindowType)
            {
                case Model.AddLogWindowType.Text:
                    _viewService?.Open(IViewService.WindowType.AddLogText);
                    break;
                case Model.AddLogWindowType.Table:
                    _viewService?.Open(IViewService.WindowType.AddLogTable);
                    break;
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
            _viewService?.Open(IViewService.WindowType.Config);
        }
        #endregion
    }
}
