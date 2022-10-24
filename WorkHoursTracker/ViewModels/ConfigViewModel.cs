using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using ProCode.WorkHoursTracker.Views;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {
        #region Fields
        ObservableCollection<RegistryPropertyViewModel> _parameters;
        IViewService _viewService;
        #endregion

        #region Constructors
        public ConfigViewModel(IViewService viewService)
        {
            _viewService = viewService;
            SaveConfigCommand = new RelayCommand(SaveExecute, SaveCanExecute);
            CancelCommand = new RelayCommand(CancelExecute);
            _parameters = new ObservableCollection<RegistryPropertyViewModel>();
            PopulateParameters();
        }
        #endregion

        #region Properties
        public bool StartWithWindows
        {
            get { return Model.Config.StartWithWindowsFlag; }
            set
            {
                Model.Config.StartWithWindowsFlag = value;
                OnPropertyChanged();
            }
        }
        public ICommand SaveConfigCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ObservableCollection<RegistryPropertyViewModel> Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        private void SaveExecute(object sender)
        {
            Model.Config.SetRegistryProperties(_parameters.Select(x => new Model.RegistryProperty
            {
                Name = x.Name,
                Description = x.Description,
                Value = x.Value
            }).ToList());
            Model.Config.Save();
            _viewService?.Close();
        }
        private bool SaveCanExecute(object obj)
        {
            return true;
        }
        private void CancelExecute(object sender)
        {
            _viewService?.Close();
        }
        private void PopulateParameters()
        {
            _parameters.Clear();
            foreach (var regProp in Model.Config.GetVisibleRegistryProperties())
            {
                RegistryPropertyViewModel regPropViewModel = new()
                {
                    Name = regProp.Name,
                    Value = regProp.Value,
                    Description = regProp.Description,
                };

                // Special case.
                switch (regPropViewModel.Name)
                {
                    case nameof(Model.Config.WorkHoursDirectory):
                        regPropViewModel.ButtonText = "📁";
                        break;
                    case nameof(Model.Config.WorkHourStart):
                    case nameof(Model.Config.WorkHourEnd):
                        regPropViewModel.ButtonText = "⏰";
                        break;
                }
                _parameters.Add(regPropViewModel);
            }
        }
        #endregion
    }
}
