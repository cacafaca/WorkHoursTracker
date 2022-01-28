using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class AddLogViewModel : BaseViewModel
    {
        #region Fields
        private string _log;
        private bool _isLoaded;
        private string _originalLog = string.Empty;
        #endregion

        #region Constructors
        public AddLogViewModel()
        {
            OpenConfigCommand = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
            SaveLogCommand = new RelayCommand(SaveLogExecute, SaveLogCanExecute);
            CancelLogCommand = new RelayCommand(CancelLogExecute, new Func<object, bool>((obj) => true));
            _isLoaded = false;

            // This needs to be the last command.
            Task.Run(() =>
            {
                ReadLog();
            });
        }
        #endregion

        #region Properties
        public string Log
        {
            get
            {
                return _log;
            }
            set
            {
                var oldLog = _log;
                _log = value;
                OnPropertyChanged();
                if (_log != oldLog)     // Allow Save button if Log value is changed.
                {
                    OnPropertyChanged(nameof(SaveLogCanExecuteFlag));
                }
            }
        }
        public bool IsLoaded { get { return _isLoaded; } }
        public bool IsLoading { get { return !_isLoaded; } }
        public IWindowFactory ConfigWindowFactory { get; set; }
        public bool SaveLogCanExecuteFlag { get { return SaveLogCanExecute(null); } }
        public ICommand OpenConfigCommand { get; set; }
        public ICommand SaveLogCommand { get; set; }
        public ICommand CancelLogCommand { get; set; }
        #endregion

        #region Methods
        private void ReadLog()
        {
            if (!_isLoaded)
                try
                {
                    WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel(Model.Config.WorkHoursCurrentFilePath);
                    whExcel.Read();
                    _isLoaded = true;
                    _originalLog = whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log ?? string.Empty;
                    _log = _originalLog;
                    OnPropertyChanged(nameof(Log));
                    OnPropertyChanged(nameof(IsLoaded));
                    OnPropertyChanged(nameof(IsLoading));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
        }
        private void ConfigExecute(object sender)
        {
            if (ConfigWindowFactory != null)
            {
                if (!ConfigWindowFactory.IsCreated())
                    ConfigWindowFactory.CreateWindow();
                if (!ConfigWindowFactory.IsVisible())
                    ConfigWindowFactory.ShowWindow();
            }
        }
        private void SaveLogExecute(object sender)
        {
            // Fire (saving) and forget.
            Task.Run(() =>
            {
                try
                {
                    WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel(Model.Config.WorkHoursCurrentFilePath);
                    Trace.WriteLine($"Read from '{whExcel.WorkHoursExcelFilePath}'.");
                    whExcel.Read();
                    whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log = Log;
                    Trace.WriteLine($"Log value: '{whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log}'.");
                    Trace.WriteLine($"Write to '{whExcel.WorkHoursExcelFilePath}' ...");
                    whExcel.Write();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.StackTrace);
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });

            InvokeClosingEvent();
        }
        private bool SaveLogCanExecute(object obj)
        {
            return _originalLog != (Log ?? string.Empty);
        }
        private void CancelLogExecute(object sender)
        {
            InvokeClosingEvent();
        }
        #endregion
    }
}
