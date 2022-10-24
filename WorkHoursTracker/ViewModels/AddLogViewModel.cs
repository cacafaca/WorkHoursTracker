using ProCode.WorkHoursTracker.Views;
using System;
using System.Collections.ObjectModel;
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
        private string _log;
        private bool _isLoaded;
        private string _originalLog = string.Empty;
        private IViewService _viewService;
        #endregion

        #region Constructors
        public AddLogViewModel(IViewService viewService)
        {
            _viewService = viewService;
            SaveLogCommand = new RelayCommand(SaveLogExecute, SaveLogTextCanExecute);
            CancelLogCommand = new RelayCommand(CancelLogExecute);
            OpenConfigCommand = new RelayCommand(OpenConfigExecute);
            PasteCommand = new RelayCommand(PasteExecute, PasteCanExecute);
            PasteFormatedCommand = new RelayCommand(PasteFormatedExecute, PasteCanExecute);
            SwitchToTableViewCommand = new RelayCommand(SwitchToTableViewExecute, SwitchToTableViewCanExecute);
            SwitchToTextViewCommand = new RelayCommand(SwitchToTextViewExecute, SwitchToTextViewCanExecute);

            _isLoaded = false;

            // This needs to be the last command.
            Task.Run(() =>
            {
                ReadLog();
            });
        }
        #endregion

        #region Events
        public event EventHandler AfterPaste;
        #endregion

        #region Properties
        public string LogText
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
                    OnPropertyChanged(nameof(SaveLogTextCanExecuteFlag));
                }
            }
        }
        public ObservableCollection<Model.LogTableRow> LogTable
        {
            get
            {
                ObservableCollection<Model.LogTableRow> logTable = new ObservableCollection<Model.LogTableRow>();
                Regex regex = new Regex(regexSpentHours, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                if (_log != null)
                    try
                    {
                        foreach (var row in _log.Split(GetSeparator()))
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
                                logTable.Add(new Model.LogTableRow { Task = task, Spent = spent });
                            }
                        }
                    }
                    catch
                    {
                        if (logTable.Count == 0)
                            logTable.Add(new Model.LogTableRow { Task = _log, Spent = 0 });
                    }
                return logTable;
            }
            set
            {
                _log = String.Join(GetSeparator(), value);
                OnPropertyChanged();
            }
        }
        public bool IsLoaded { get { return _isLoaded; } }
        public bool IsLoading { get { return !_isLoaded; } }
        public bool SaveLogTextCanExecuteFlag { get { return SaveLogTextCanExecute(null); } }
        public bool SaveLogTableCanExecuteFlag { get { return SaveLogTextCanExecute(null); } }

        public ICommand SaveLogCommand { get; set; }
        public ICommand CancelLogCommand { get; set; }
        public ICommand OpenConfigCommand{ get; set; }        
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
                    _log = _originalLog;
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
        private bool SaveLogTextCanExecute(object obj)
        {
            return _originalLog != (LogText ?? string.Empty);
        }
        private void CancelLogExecute(object sender)
        {
            _viewService?.Close();
        }
        private void OpenConfigExecute(object sender)
        {
            _viewService.Open(IViewService.WindowType.Config);
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
            Model.Config.AddLogWindowType = Model.AddLogWindowType.Text;
            Model.Config.Save(nameof(Model.Config.AddLogWindowType));
            _viewService?.Open(IViewService.WindowType.AddLogText, this);
        }
        private bool SwitchToTextViewCanExecute(object obj)
        {
            return true;
        }
        #endregion
    }
}
