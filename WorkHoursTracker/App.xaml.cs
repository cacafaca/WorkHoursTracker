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
        private TaskbarIcon _notifyIcon;
        private System.Windows.Threading.DispatcherTimer _repeatingTimer = new System.Windows.Threading.DispatcherTimer();
        private System.Windows.Threading.DispatcherTimer _beforeEndTimer = new System.Windows.Threading.DispatcherTimer();
        private EventHandler _onTickEventHandler;
        private EventHandler _onBeforeEndHandler;
        private Icon _baloonIcon;
        #endregion

        #region Methods
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            if (_notifyIcon != null && _notifyIcon.DataContext != null && _notifyIcon.DataContext is ViewModels.NotifyIconViewModel)
            {
                // Set Log window type.
                ((ViewModels.NotifyIconViewModel)_notifyIcon.DataContext).AddLogWindowFactory =
                    new Views.BaseWindowFactory(typeof(Views.AddLogView));

                // Set Config window type.
                ((ViewModels.NotifyIconViewModel)_notifyIcon.DataContext).ConfigWindowFactory =
                    new Views.BaseWindowFactory(typeof(Views.ConfigView));

                Model.Config.ConfigSaved += OnConfigSavedHndler;
                _notifyIcon.TrayBalloonTipClicked += NotifyIcon_TrayBalloonTipClicked;

                _baloonIcon = WorkHoursTracker.Properties.Resources.App;

                InitTimers();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();      // The icon would clean up automatically, but this is cleaner.
            _repeatingTimer.Stop();    // Stop the timer, in any case.
            _beforeEndTimer.Stop();

            base.OnExit(e);
        }

        private void NotifyIcon_TrayBalloonTipClicked(object sender, RoutedEventArgs e)
        {
            (_notifyIcon.DataContext as ViewModels.NotifyIconViewModel).AddLogCommand.Execute(this);
        }

        private void OnConfigSavedHndler(EventArgs e)
        {
            InitTimers();
        }

        private void InitTimers()
        {
            // Regular timer.
            if (_onTickEventHandler != null)
                _repeatingTimer.Tick -= _onTickEventHandler;       // Remove old handler, to keep clean.
            _onTickEventHandler = new EventHandler(OnTickNotify);
            _repeatingTimer.Tick += _onTickEventHandler;
#if !DEBUG
            _repeatingTimer.Interval = new TimeSpan(0, (int)Model.Config.TimerIntervalInMinutes, 0);
#else
            _repeatingTimer.Interval = new TimeSpan(0, 0, 15);
#endif
            InitBeforeEndTimer();
            _repeatingTimer.Start();
            _beforeEndTimer.Start();
        }

        private void InitBeforeEndTimer()
        {
            // Before end timer.
            if (_onBeforeEndHandler != null)
                _beforeEndTimer.Tick -= _onBeforeEndHandler;        // Remove old handler, to keep clean.
            _onBeforeEndHandler = new EventHandler(OnTickNotify);
            _beforeEndTimer.Tick += _onBeforeEndHandler;

            _beforeEndTimer.Interval = Model.Config.WorkHourEndAsDateTime.AddMinutes(-Model.Config.RemindBeforeWorkHoursEndInMinutes)
                .AddDays(Convert.ToInt32(Model.Config.WorkHourEndAsDateTime.AddMinutes(-Model.Config.RemindBeforeWorkHoursEndInMinutes) < DateTime.Now)) - DateTime.Now;
        }

        private bool CanTriggerExecution()
        {
            return !(DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)   // Don't popup on weekends. Who works on weekends?!
                && DateTime.ParseExact(Model.Config.WorkHourStart, "HH:mm", System.Globalization.CultureInfo.InvariantCulture) <= DateTime.Now
                && DateTime.Now <= DateTime.ParseExact(Model.Config.WorkHourEnd, "HH:mm", System.Globalization.CultureInfo.InvariantCulture)
                && _notifyIcon != null && _notifyIcon.DataContext != null
                && _notifyIcon.DataContext is ViewModels.NotifyIconViewModel
                && !((ViewModels.NotifyIconViewModel)_notifyIcon.DataContext).AddLogWindowFactory.IsCreated();          // Don't popup if Add Lof windows is already open. It will annoy.

        }
        private void OnTickNotify(object? sender, EventArgs e)
        {
            InitBeforeEndTimer();
            if (CanTriggerExecution())
            {
                _notifyIcon.ShowBalloonTip(
                    title: "Add work hours log.",
                    message: $"What did you do last {Model.Config.TimerIntervalInMinutes} minute{new string('s', Convert.ToInt32(Math.Ceiling((double)(Model.Config.TimerIntervalInMinutes - 1) / (double)uint.MaxValue)))}?\nClick to add log.",
                    customIcon: _baloonIcon,
                    largeIcon: true
                    //BalloonIcon.Info
                    );
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
        #endregion

    }
}
