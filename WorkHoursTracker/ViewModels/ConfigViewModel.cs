using System.Windows.Input;
using System.Windows.Forms;
using System.Windows;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {
        const string workHoursDirecorySettingsName = "WorkHoursDirectory";

        string workHoursDirecoryOld;
        string workHoursDirecory;
        public string WorkHoursDirectory
        {
            get
            {
                return workHoursDirecory;
            }
            set
            {
                workHoursDirecoryOld = workHoursDirecory;
                workHoursDirecory = value;
                OnPropertyChanged();
            }
        }

        public ConfigViewModel()
        {
            WorkHoursDirectory = Properties.Settings.Default[workHoursDirecorySettingsName].ToString();
            SetWorkHoursDirCommand = new RelayCommand(SetWorkingDir, CanSetWorkingDir);
            SaveCommand = new RelayCommand(Save, CanSave);
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
        public ICommand SaveCommand { get; set; }
        private void Save(object sender)
        {
            Properties.Settings.Default[workHoursDirecorySettingsName] = WorkHoursDirectory;
            Properties.Settings.Default.Save();
            if (sender is Window)
            {
                ((Window)sender).Close();
            }
        }
        private bool CanSave(object obj)
        {
            return workHoursDirecory != workHoursDirecoryOld;
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
