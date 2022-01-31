using System.Windows.Input;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {
        #region Constructors
        public ConfigViewModel()
        {
            SetWorkHoursDirCommand = new RelayCommand(SetWorkingDir, CanSetWorkingDir);
            SaveConfigCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }
        #endregion

        #region Properties
        public string WorkHoursDirectory
        {
            get
            {
                return Model.Config.WorkHoursDirectory;
            }
            set
            {
                Model.Config.WorkHoursDirectory = value;
                OnPropertyChanged();
            }
        }
        public int TimerIntervalInMinutes
        {
            get { return Model.Config.TimerIntervalInMinutes; }
            set
            {
                Model.Config.TimerIntervalInMinutes = value;
                OnPropertyChanged();
            }
        }
        public bool StartWithWindows
        {
            get { return Model.Config.StartWithWindowsFlag; }
            set
            {
                Model.Config.StartWithWindowsFlag = value;
                OnPropertyChanged();
            }
        }
        public ICommand SetWorkHoursDirCommand { get; set; }
        public ICommand SaveConfigCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ObservableCollection<RegistryPropertyViewModel> Parameters
        {
            get
            {
                ObservableCollection<RegistryPropertyViewModel> parameters = new();
                foreach (var regProp in Model.Config.GetRegistryProperties())
                {
                    RegistryPropertyViewModel regPropViewModel = new()
                    {
                        Name = regProp.Name,
                        Value = regProp.Value,
                        Description = regProp.Description
                    };
                    regPropViewModel.PropertyChanged += RegPropViewModel_PropertyChanged;
                    parameters.Add(regPropViewModel);
                }
                return parameters;
            }
            set
            {
                Model.Config.SetRegistryProperties(value
                    .Select(x => new Model.RegistryProperty
                    {
                        Name = x.Name,
                        Description = x.Description,
                        Value = x.Value
                    }).ToList());
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        private void SetWorkingDir(object obj)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.InitialDirectory = WorkHoursDirectory;
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                    WorkHoursDirectory = dialog.SelectedPath;
            }
        }
        private bool CanSetWorkingDir(object obj)
        {
            return true;
        }
        private void Save(object sender)
        {
            Model.Config.Save();
            InvokeClosingEvent();
        }
        private bool CanSave(object obj)
        {
            return true;
        }
        private void Cancel(object sender)
        {
            InvokeClosingEvent();
        }
        private bool CanCancel(object obj)
        {
            return true;
        }
        private void RegPropViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is RegistryPropertyViewModel propertyViewModel)
                Model.Config.SetRegistryProperty(new Model.RegistryProperty
                {
                    Name = propertyViewModel.Name,
                    Description = propertyViewModel.Description,
                    Value = propertyViewModel.Value
                });
        }
        #endregion


    }
}
