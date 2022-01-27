using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class AddLogViewModel : BaseViewModel
    {
        #region Fields
        private string _log;
        private System.Windows.Threading.DispatcherTimer _closeTimer;
        private bool _isLoaded;
        private string _originalLog = string.Empty;
        #endregion

        #region Constructors
        public AddLogViewModel()
        {
            OpenConfigCommand = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
            SaveLogCommand = new RelayCommand(SaveLogExecute, SaveLogCanExecute);
            CancelLogCommand = new RelayCommand(CancelLogExecute, new Func<object, bool>((obj) => true));
            StartCloseTimerCommand = new RelayCommand(StartCloseTimerExecute, new Func<object, bool>((obj) => true));
            _isLoaded = false;
        }
        #endregion

        #region Properties
        public string Log
        {
            get
            {
                return ReadLog();
            }
            set
            {
                var oldLog = _log;
                _log = value;
                OnPropertyChanged();
                if (_log != oldLog)
                {
                    OnPropertyChanged(nameof(SaveLogCanExecuteFlag));
                    StopCloseTimer();
                }
            }
        }
        public IWindowFactory ConfigWindowFactory { get; set; }
        public bool SaveLogCanExecuteFlag { get { return SaveLogCanExecute(null); } }
        public ICommand OpenConfigCommand { get; set; }
        public ICommand SaveLogCommand { get; set; }
        public ICommand CancelLogCommand { get; set; }
        public ICommand StartCloseTimerCommand { get; set; }
        #endregion

        #region Methods
        private void StartCloseTimerExecute(object obj)
        {
            InitCloseTimer();
        }
        private string ReadLog()
        {
            if (_isLoaded)
            {
                return _log;
            }
            else
            {
                try
                {
                    WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel(Model.Config.WorkHoursCurrentFilePath);
                    whExcel.Read();
                    _isLoaded = true;
                    _originalLog = whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log ?? string.Empty;
                    return _originalLog;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    return ex.Message;
                }
            }
        }
        private void ConfigExecute(object sender)
        {
            if (ConfigWindowFactory != null)
            {
                ConfigWindowFactory.CreateWindow();
                ConfigWindowFactory.ShowWindow();
            }
        }
        private void InitCloseTimer()
        {
            _closeTimer = new System.Windows.Threading.DispatcherTimer();
            _closeTimer.Tick += new EventHandler(OnTick);
            _closeTimer.Interval = new TimeSpan(0, 0, (int)Model.Config.VisibilityIntervalInSeconds);
            _closeTimer.Start();
            Trace.WriteLine("Close timer started.");
        }
        private void StopCloseTimer()
        {
            if (_closeTimer != null)
            {
                _closeTimer.Stop();
                _closeTimer = null;
            }
        }
        private void OnTick(object? sender, EventArgs e)
        {
            Trace.WriteLine("Close timer elapsed.");
            StopCloseTimer();
            if (DefaultWindowFactory != null)
            {
                DefaultWindowFactory.CloseWindow();
            }
        }
        #endregion

        #region Save commnad
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

            if (DefaultWindowFactory != null)
            {
                DefaultWindowFactory.CloseWindow();
            }
        }
        private bool SaveLogCanExecute(object obj)
        {
            return _originalLog != (Log ?? string.Empty);
        }
        private void CancelLogExecute(object sender)
        {
            if (DefaultWindowFactory != null)
            {
                DefaultWindowFactory.CloseWindow();
            }
        }
        #endregion
    }
}
