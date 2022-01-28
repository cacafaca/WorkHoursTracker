using System.Windows.Input;
using System.Windows.Forms;

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
        public uint TimerIntervalInMinutes
        {
            get { return Model.Config.TimerIntervalInMinutes; }
            set
            {
                Model.Config.TimerIntervalInMinutes = value;
                OnPropertyChanged();
            }
        }
        #endregion

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
            InvokeClosingEvent();
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
            InvokeClosingEvent();
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
