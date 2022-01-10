using System.Windows.Input;
using System.Windows.Forms;
using System.Windows;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {
        Model.Config _config;

        string workHoursDirecoryOld;
        public string WorkHoursDirectory
        {
            get
            {
                return _config.WorkHoursDirectory;
            }
            set
            {
                workHoursDirecoryOld = _config.WorkHoursDirectory;
                _config.WorkHoursDirectory = value;
                OnPropertyChanged();
            }
        }

        public ConfigViewModel()
        {
            _config = new Model.Config();

            SetWorkHoursDirCommand = new RelayCommand(SetWorkingDir, CanSetWorkingDir);
            SaveConfigCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        #region Direcotry dialog command
        public ICommand SetWorkHoursDirCommand { get; set; }

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
        #endregion

        #region OK/Save command
        public ICommand SaveConfigCommand { get; set; }
        private void Save(object sender)
        {
            _config.Save();
            if (sender is Window)
            {
                ((Window)sender).Close();
            }
        }
        private bool CanSave(object obj)
        {
            return _config.WorkHoursDirectory != workHoursDirecoryOld;
        }
        #endregion

        #region Cancel commnad
        public ICommand CancelCommand { get; set; }
        private void Cancel(object sender)
        {
            if (sender is Window)
                ((Window)sender).Close();
        }
        private bool CanCancel(object obj)
        {
            return true;
        }
        #endregion
    }
}
