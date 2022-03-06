using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows;

namespace ProCode.WorkHoursTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields
        private TaskbarIcon _notifyAddLogIcon;
        private System.Windows.Threading.DispatcherTimer _addLogTimer = new System.Windows.Threading.DispatcherTimer();
        private System.Windows.Threading.DispatcherTimer _beforeEndTimer = new System.Windows.Threading.DispatcherTimer();
        private System.Windows.Threading.DispatcherTimer _confirmSentReportTimer = new System.Windows.Threading.DispatcherTimer();
        private EventHandler _onTickEventHandler;
        private EventHandler _onBeforeEndHandler;
        private EventHandler _onConfirmSendReportHandler;
        private Icon _baloonIcon;
        private BalloonType _ballonType = BalloonType.Unselected;
        private bool _isConfirmSentReportDialogShown = false;
        #endregion

        #region Enums
        enum BalloonType
        {
            Unselected,
            AddLog,
            SendReportConfirmation
        }
        #endregion

        #region Methods
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyAddLogIcon = (TaskbarIcon)FindResource("NotifyIcon");
            if (_notifyAddLogIcon != null && _notifyAddLogIcon.DataContext != null && _notifyAddLogIcon.DataContext is ViewModels.NotifyIconViewModel)
            {
                // Set Log window type.
                ((ViewModels.NotifyIconViewModel)_notifyAddLogIcon.DataContext).AddLogWindowFactory =
                    new Views.BaseWindowFactory(typeof(Views.AddLogView));

                // Set Config window type.
                ((ViewModels.NotifyIconViewModel)_notifyAddLogIcon.DataContext).ConfigWindowFactory =
                    new Views.BaseWindowFactory(typeof(Views.ConfigView));

                Model.Config.ConfigSaved += OnConfigSavedHndler;
                _notifyAddLogIcon.TrayBalloonTipClicked += NotifyIcon_TrayBalloonTipClicked;

                _baloonIcon = WorkHoursTracker.Properties.Resources.App;

                InitTimers();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyAddLogIcon.Dispose();          // The icon would clean up automatically, but this is cleaner.
            _addLogTimer.Stop();            // Stop the timer, in any case.
            _beforeEndTimer.Stop();
            _confirmSentReportTimer.Stop();

            base.OnExit(e);
        }

        private void NotifyIcon_TrayBalloonTipClicked(object sender, RoutedEventArgs e)
        {
            switch (_ballonType)
            {
                case BalloonType.AddLog:
                    (_notifyAddLogIcon.DataContext as ViewModels.NotifyIconViewModel).AddLogCommand.Execute(this);
                    break;
                case BalloonType.SendReportConfirmation:
                    ConfirmSendReport();
                    break;
            }
            _ballonType = BalloonType.Unselected;
        }

        private void OnConfigSavedHndler(EventArgs e)
        {
            InitTimers();
        }

        private void InitTimers()
        {
            InitAddLogTimer();
            InitBeforeWorkHoursEndTimer();
            InitSendReportTimer();

            _addLogTimer.Start();
            _beforeEndTimer.Start();
            _confirmSentReportTimer.Start();
        }

        private void InitAddLogTimer()
        {
            // Add Log timer.
            if (_onTickEventHandler != null)
                _addLogTimer.Tick -= _onTickEventHandler;       // Remove old handler, to keep clean.
            _onTickEventHandler = new EventHandler(OnAddLogTickNotify);
            _addLogTimer.Tick += _onTickEventHandler;
#if !DEBUG
            _addLogTimer.Interval = new TimeSpan(0, (int)Model.Config.TimerIntervalInMinutes, 0);       // Production value.
#else       
            _addLogTimer.Interval = new TimeSpan(0, 0, 15);                                             // Debug value = 15 seconds.
#endif
        }

        private void InitBeforeWorkHoursEndTimer()
        {
            // Before end timer.
            if (_onBeforeEndHandler != null)
                _beforeEndTimer.Tick -= _onBeforeEndHandler;        // Remove old handler, to keep clean.
            _onBeforeEndHandler = new EventHandler(OnAddLogTickNotify);
            _beforeEndTimer.Tick += _onBeforeEndHandler;

            _beforeEndTimer.Interval = Model.Config.WorkHourEndAsDateTime.AddMinutes(-Model.Config.RemindBeforeWorkHoursEndInMinutes)
                .AddDays(Convert.ToInt32(Model.Config.WorkHourEndAsDateTime.AddMinutes(-Model.Config.RemindBeforeWorkHoursEndInMinutes) < DateTime.Now)) - DateTime.Now;
        }

        public void InitSendReportTimer()
        {
            if (_onConfirmSendReportHandler != null)
                _confirmSentReportTimer.Tick -= _onConfirmSendReportHandler;        // Remove old handler, to keep clean.
            _onConfirmSendReportHandler = new EventHandler(OnSendReportTickNotify);
            _confirmSentReportTimer.Tick += _onConfirmSendReportHandler;
#if !DEBUG
            _confirmSentReportTimer.Interval = new TimeSpan(1, 0, 0);       // Production value.
#else
            _confirmSentReportTimer.Interval = new TimeSpan(0, 0, 25);      // Debug value = 25 seconds.
#endif
        }

        /// <summary>
        /// Calculate if notify balloon tip should be shown.
        /// </summary>
        /// <returns></returns>
        private bool CanTriggerExecution()
        {
            return !(DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)   // Don't pop-up on weekends. Who works on weekends?!
                && Model.Config.WorkHourStartAsDateTime <= DateTime.Now
                && DateTime.Now <= Model.Config.WorkHourEndAsDateTime
                && _notifyAddLogIcon != null && _notifyAddLogIcon.DataContext != null
                && _notifyAddLogIcon.DataContext is ViewModels.NotifyIconViewModel
                && !((ViewModels.NotifyIconViewModel)_notifyAddLogIcon.DataContext).AddLogWindowFactory.IsCreated();          // Don't pop-up if Add Log windows is already open. It will annoy.
        }

        private void OnAddLogTickNotify(object? sender, EventArgs e)
        {
            InitBeforeWorkHoursEndTimer();
            if (CanTriggerExecution())
            {
                _notifyAddLogIcon.ShowBalloonTip(
                    title: "Add work hours log.",
                    message: $"What did you do last {Model.Config.TimerIntervalInMinutes} minute{new string('s', Convert.ToInt32(Math.Ceiling((double)(Model.Config.TimerIntervalInMinutes - 1) / (double)uint.MaxValue)))}?\nClick to add log.",
                    customIcon: _baloonIcon,
                    largeIcon: true
                    //BalloonIcon.Info
                    );
                _ballonType = BalloonType.AddLog;
            }
        }

        /// <summary>
        /// We'll have two reminders per day for sending report, for dates within deadline.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSendReportTickNotify(object? sender, EventArgs e)
        {
            if (!_isConfirmSentReportDialogShown)
            {
                // First check is to compare the date from registry.
                if (Model.Config.LastResponseSentDateAsDateTime.Year < DateTime.Now.Year
                    || Model.Config.LastResponseSentDateAsDateTime.Month < DateTime.Now.Month)
                {
                    TimeSpan halfOfInterval = _confirmSentReportTimer.Interval.Divide(2);
                    TimeSpan thirdOfWorkHours = (Model.Config.WorkHourEndAsDateTime - Model.Config.WorkHourStartAsDateTime).Divide(3);
                    TimeSpan check1 = thirdOfWorkHours;
                    TimeSpan check2 = thirdOfWorkHours.Multiply(2);
                    TimeSpan currentTimeSpan = DateTime.Now - Model.Config.WorkHourStartAsDateTime;

                    // Check if current date is within the deadline range. Then remind two times a day. Otherwise remind every hour.
                    if ((DateTime.Today <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, Model.Config.ReportDeadlineIndays) &&
                            (check1.Subtract(halfOfInterval) < currentTimeSpan && currentTimeSpan <= check1.Add(halfOfInterval) ||
                             check2.Subtract(halfOfInterval) < currentTimeSpan && currentTimeSpan <= check2.Add(halfOfInterval)))
                        ||
                        (new DateTime(DateTime.Now.Year, DateTime.Now.Month, Model.Config.ReportDeadlineIndays) < DateTime.Today))
                    {
                        _notifyAddLogIcon.ShowBalloonTip(
                        title: "Send Report",
                        message: $"Did you send the work hours report?\nClick to confirm.",
                        customIcon: _baloonIcon,
                        largeIcon: true
                        );
                        _ballonType = BalloonType.SendReportConfirmation;
                    }
                }
            }
        }

        /// <summary>
        /// Just display error if we don't know how to handle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception occurred:\n" + e.Exception.Message + "\n" + e.Exception.InnerException?.Message);
            e.Handled = true;
        }

        private void ConfirmSendReport()
        {
            _isConfirmSentReportDialogShown = true;
            if (MessageBox.Show("Did you sent working hours report?\n\"No\" will remind you again.", "Send report reminder", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Model.Config.SetLastReportSentDate(DateTime.Now);
            }
            _isConfirmSentReportDialogShown = false;
        }
        #endregion

    }
}
