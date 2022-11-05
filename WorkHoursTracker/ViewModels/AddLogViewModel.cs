using Microsoft.VisualBasic.Logging;
using ProCode.WorkHoursTracker.Model;
using ProCode.WorkHoursTracker.ViewModels;
using ProCode.WorkHoursTracker.Views;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class AddLogViewModel : BaseViewModel
    {
        #region Constants
        string regexSpentHours = @"\[\d*\.?\d*\]$";
        #endregion

        #region Fields
        private string? _logText;
        private bool _isLoaded;
        private string _originalLog = string.Empty;
        private IViewService? _viewService;
        private ObservableCollection<LogTableRowViewModel> _logTable;
        private bool _isRecreating = false;
        #endregion

        #region Constructors
        public AddLogViewModel(IViewService? viewService)
        {
            _viewService = viewService;

            _logTable = new ObservableCollection<LogTableRowViewModel>();
            _logTable.CollectionChanged += NotifyLogTableChanged;

            SaveLogCommand = new RelayCommand(SaveLogExecute, SaveLogCanExecute);
            CancelLogCommand = new RelayCommand(CancelLogExecute);
            OpenConfigCommand = new RelayCommand(OpenConfigExecute);
            PasteCommand = new RelayCommand(PasteExecute, PasteCanExecute);
            PasteFormatedCommand = new RelayCommand(PasteFormatedExecute, PasteCanExecute);
            SwitchToTableViewCommand = new RelayCommand(SwitchToTableViewExecute, SwitchToTableViewCanExecute);
            SwitchToTextViewCommand = new RelayCommand(SwitchToTextViewExecute, SwitchToTextViewCanExecute);

            _isLoaded = false;

            // This needs to be the last command, so I can fire and forget.
            Task.Run(() =>
            {
                ReadLog();
            });
        }
        public AddLogViewModel() : this(null) { }
        #endregion

        #region Events
        public event EventHandler? AfterPaste;
        #endregion

        #region Properties
        public string LogText
        {
            get { return _logText; }
            set
            {
                var oldLog = _logText;
                _logText = value;
                OnPropertyChanged();
                RecreateLogTable(true);
                if (_logText != oldLog)     // Allow Save button if Log value is changed.
                {
                    OnPropertyChanged(nameof(SaveLogCanExecuteFlag));
                }
            }
        }
        public ObservableCollection<LogTableRowViewModel> LogTable
        {
            get
            {
                if (_logTable.Count == 0 && !string.IsNullOrEmpty(_logText))
                {
                    _isRecreating = true;
                    RecreateLogTable();
                    _isRecreating = false;
                }

                return _logTable;
            }
            set
            {
                var oldLog = _logText;
                _logTable = value;
                _logText = String.Join(GetSeparator(), value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(LogText));
                if (_logText != oldLog)     // Allow Save button if Log value is changed.
                {
                    OnPropertyChanged(nameof(SaveLogCanExecuteFlag));
                }
            }
        }

        private string _headerColumnSpent;

        public string HeaderColumnSpent
        {
            get { return _headerColumnSpent; }
            private set
            {
                _headerColumnSpent = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoaded { get { return _isLoaded; } }
        public bool IsLoading { get { return !_isLoaded; } }
        public bool SaveLogCanExecuteFlag { get { return SaveLogCanExecute(null); } }

        public ICommand SaveLogCommand { get; set; }
        public ICommand CancelLogCommand { get; set; }
        public ICommand OpenConfigCommand { get; set; }
        public ICommand PasteCommand { get; set; }
        public ICommand PasteFormatedCommand { get; set; }
        public ICommand SwitchToTableViewCommand { get; set; }         // Opens Add Log as table view
        public ICommand SwitchToTextViewCommand { get; set; }          // Opens Add log as default (text) view.
        #endregion

        #region Methods
        private void ReadLog()
        {
            if (!_isLoaded)
                try
                {
                    WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel(Model.Config.WorkHoursCurrentFilePath);
                    whExcel.Read();
                    _originalLog = whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log ?? string.Empty;
                    _isLoaded = true;
                    _logText = _originalLog;
                    //RecreateLogTable();
                    OnPropertyChanged(nameof(LogText));
                    OnPropertyChanged(nameof(LogTable));
                    OnPropertyChanged(nameof(IsLoaded));
                    OnPropertyChanged(nameof(IsLoading));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
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
                    whExcel.WorkHours.Where(wh => wh.Date == DateOnly.FromDateTime(DateTime.Now)).First().Log = LogText;
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
            _viewService?.Close();
        }

        private bool SaveLogCanExecute(object obj)
        {
            return _originalLog != (LogText ?? string.Empty);
        }

        private void CancelLogExecute(object sender)
        {
            _viewService?.Close();
        }

        private void OpenConfigExecute(object sender)
        {
            _viewService?.Open(IViewService.WindowType.Config);
        }

        private bool PasteCanExecute(object arg)
        {
            return !string.IsNullOrWhiteSpace(System.Windows.Clipboard.GetText());
        }

        private void PasteExecute(object obj)
        {
            LogText += System.Windows.Clipboard.GetText().Trim();
            AfterPaste?.Invoke(this, new EventArgs());
        }

        private void PasteFormatedExecute(object obj)
        {
            LogText = LogText.TrimEnd() + AddFromClipboard() + GetSeparator();
            AfterPaste?.Invoke(this, new EventArgs());
        }

        private string AddFromClipboard()
        {
            string logTrimmed = LogText.TrimEnd();
            string addition = string.Empty;
            if (logTrimmed.Length > 0)
            {
                if (!logTrimmed.EndsWith(GetSeparator()))
                    addition = GetSeparator();
                addition += " ";
            }

            return addition + System.Windows.Clipboard.GetText().Trim().TrimEnd(GetSeparator().ToArray());
        }

        private string GetSeparator()
        {
            return Model.Config.Separator;
        }

        private void SwitchToTableViewExecute(object obj)
        {
            _viewService?.Close();
            Model.Config.AddLogWindowType = Model.AddLogWindowType.Table;
            Model.Config.Save(nameof(Model.Config.AddLogWindowType));
            _viewService?.Open(IViewService.WindowType.AddLogTable, this);
        }

        private bool SwitchToTableViewCanExecute(object obj)
        {
            return true;
        }

        private void SwitchToTextViewExecute(object obj)
        {
            _viewService?.Close();
            Config.AddLogWindowType = Model.AddLogWindowType.Text;
            Model.Config.Save(nameof(Model.Config.AddLogWindowType));
            _viewService?.Open(IViewService.WindowType.AddLogText, this);
        }

        private bool SwitchToTextViewCanExecute(object obj)
        {
            return true;
        }

        private void NotifyLogTableChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<LogTableRowViewModel> logTable)
            {
                Trace.WriteLine("Count: " + logTable.Count);
            }
            if (e.NewItems != null)
                foreach (LogTableRowViewModel logTableRow in e.NewItems)
                    logTableRow.PropertyChanged += LogTableRow_PropertyChanged;
            if (e.OldItems != null)
                foreach (LogTableRowViewModel logTableRow in e.OldItems)
                    logTableRow.PropertyChanged -= LogTableRow_PropertyChanged;

            if (!_isRecreating)
            {
                RecreateLogText();
                OnPropertyChanged(nameof(SaveLogCanExecuteFlag));
            }
        }

        private void LogTableRow_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RecreateLogText();
            HeaderColumnSpent = $"Spent (∑ {_logTable.Select(x=>x.Spent).Sum()})";
            OnPropertyChanged(nameof(SaveLogCanExecuteFlag));
        }

        private void RecreateLogTable(bool notify = false)
        {
            //_logTable.CollectionChanged -= NotifyLogTableChanged;
            _logTable.Clear();
            Regex regex = new Regex(regexSpentHours, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (_logText != null)
                try
                {
                    foreach (var row in _logText.Split(GetSeparator()))
                    {
                        if (!string.IsNullOrWhiteSpace(row))
                        {
                            string task = row.Trim();
                            Single spent = 0;
                            try
                            {
                                var match = regex.Match(task);
                                if (match.Success)
                                {
                                    spent = Convert.ToSingle(match.Value.Trim('[', ']'));
                                    task = task.Remove(task.Length - match.Value.Length);
                                }
                            }
                            catch
                            {
                                spent = 0;
                            }
                            _logTable.Add(new LogTableRowViewModel { Task = task, Spent = spent });
                        }
                    }
                }
                catch
                {
                    if (_logTable.Count == 0)
                        _logTable.Add(new LogTableRowViewModel { Task = _logText, Spent = 0 });
                }

            if (notify)
                OnPropertyChanged(nameof(LogTable));
        }

        private void RecreateLogText(bool notify = false)
        {
            _logText = String.Join(GetSeparator() + " ", _logTable);
            if (notify)
                OnPropertyChanged(nameof(LogText));
        }
        #endregion
    }
}
