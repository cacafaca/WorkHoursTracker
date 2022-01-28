using Hardcodet.Wpf.TaskbarNotification;
using System;
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
        private System.Windows.Threading.DispatcherTimer _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private EventHandler _onTickEventHandler;
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
                InitTimer();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();      // The icon would clean up automatically, but this is cleaner.
            _dispatcherTimer.Stop();    // Stop the timer, in any case.

            base.OnExit(e);
        }

        private void NotifyIcon_TrayBalloonTipClicked(object sender, RoutedEventArgs e)
        {
            (_notifyIcon.DataContext as ViewModels.NotifyIconViewModel).AddLogCommand.Execute(this);
        }

        private void OnConfigSavedHndler(EventArgs e)
        {
            InitTimer();
        }

        private void InitTimer()
        {
            // Remove old handler in case timer needs to be initiated again.
            if (_onTickEventHandler != null)
                _dispatcherTimer.Tick -= _onTickEventHandler;

            _onTickEventHandler = new EventHandler(OnTick);
            _dispatcherTimer.Tick += _onTickEventHandler;
#if !DEBUG
            _dispatcherTimer.Interval = new TimeSpan(0, (int)Model.Config.TimerIntervalInMinutes, 0);
#else
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 15);
#endif
            _dispatcherTimer.Start();
        }

        private void OnTick(object? sender, EventArgs e)
        {
            if (_notifyIcon != null && _notifyIcon.DataContext != null
                && _notifyIcon.DataContext is ViewModels.NotifyIconViewModel)
            {
                if (!((ViewModels.NotifyIconViewModel)_notifyIcon.DataContext).AddLogWindowFactory.IsCreated())
                    _notifyIcon.ShowBalloonTip("Add work hours log.", $"What did you do last {Model.Config.TimerIntervalInMinutes} minute{new string('s', Convert.ToInt32(Math.Ceiling((double)(Model.Config.TimerIntervalInMinutes - 1) / (double)uint.MaxValue)))}?\nClick to add log.", BalloonIcon.Info);
            }
        }
        #endregion
    }
}
