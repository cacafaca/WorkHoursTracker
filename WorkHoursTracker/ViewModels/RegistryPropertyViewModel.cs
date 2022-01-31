using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class RegistryPropertyViewModel : BaseViewModel
    {
        #region Fields
        private string _name;
        private string _description;
        private object _value;
        private string _buttonText;
        #endregion

        #region Constructors
        public RegistryPropertyViewModel()
        {
            SelectValueCommand = new RelayCommand(SelectValueExecute);
        }
        #endregion

        #region Delegates
        public delegate void PickTimeHandler(ref TimeOnly time);
        #endregion

        #region Events
        public event PickTimeHandler PickTimeEvent;
        #endregion

        #region Properties
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                _buttonText = value;
                OnPropertyChanged();
            }
        }
        public bool ButtonVisible { get { return !string.IsNullOrWhiteSpace(_buttonText); } }
        public ICommand SelectValueCommand { get; set; }
        #endregion

        #region Methods
        private void SelectValueExecute(object obj)
        {
            switch (_name)
            {
                case nameof(Model.Config.WorkHoursDirectory):
                    PickWorkHoursDirectory();
                    break;
                case nameof(Model.Config.WorkHourStart):
                case nameof(Model.Config.WorkHourEnd):
                    PickTime();
                    break;

            }
        }
        private void PickTime()
        {
            TimeOnly time = TimeOnly.FromDateTime(Convert.ToDateTime(Value.ToString()));
            if (PickTimeEvent != null)
            {
                PickTimeEvent.Invoke(ref time);
                Value = time.ToString("hh:mm");
            }
        }
        private void PickWorkHoursDirectory()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.InitialDirectory = Value.ToString();
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                    Value = dialog.SelectedPath;
            }
        }
        #endregion
    }
}
