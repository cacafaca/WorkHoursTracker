using System.Windows.Input;
using System.Windows.Forms;
using System.Windows;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {
        string _workHoursDirecoryOld;
        public string WorkHoursDirectory
        {
            get
            {
                return Model.Config.WorkHoursDirectory;
            }
            set
            {
                _workHoursDirecoryOld = Model.Config.WorkHoursDirectory;
                Model.Config.WorkHoursDirectory = value;
                OnPropertyChanged();
            }
        }

        private uint _timerIntervalOld;

        public uint TimerIntervalInMinutes
        {
            get { return Model.Config.TimerIntervalInMinutes; }
            set
            {
                _timerIntervalOld = Model.Config.TimerIntervalInMinutes;
                Model.Config.TimerIntervalInMinutes = value;
                OnPropertyChanged();
            }
        }

        private uint _visibilityIntervalOld;

        public uint VisibilityIntervalInSeconds
        {
            get { return Model.Config.VisibilityIntervalInSeconds; }
            set
            {
                _visibilityIntervalOld = Model.Config.VisibilityIntervalInSeconds;
                Model.Config.VisibilityIntervalInSeconds = value;
                OnPropertyChanged();
            }
        }

        public ConfigViewModel()
        {
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
            Model.Config.Save();
            DefaultWindowFactory.CloseWindow();
        }
        private bool CanSave(object obj)
        {
            return true;
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

        public bool StartWithWindows
        {
            get { return Model.Config.StartWithWindowsFlag; }
            set
            {
                Model.Config.StartWithWindowsFlag = value;
                OnPropertyChanged();
            }
        }

    }
}
