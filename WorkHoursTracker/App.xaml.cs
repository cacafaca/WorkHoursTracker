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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            if (_notifyIcon != null && _notifyIcon.DataContext != null &&
                _notifyIcon.DataContext is ViewModels.NotifyIconViewModel)
            {
                // Set Log window type.
                ((ViewModels.NotifyIconViewModel)_notifyIcon.DataContext).AddLogWindowFactory =
                    new Views.AddLogWindowFactory(typeof(Views.AddLogView));

                // Set Config window type.
                ((ViewModels.NotifyIconViewModel)_notifyIcon.DataContext).ConfigWindowFactory =
                    new Views.BaseWindowFactory(typeof(Views.ConfigView));

                InitTimer();
                Model.Config.ConfigSaved += OnConfigSavedHndler;
            }
        }

        private void OnConfigSavedHndler(EventArgs e)
        {
            InitTimer();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();      // The icon would clean up automatically, but this is cleaner.
            _dispatcherTimer.Stop();    // Stop the timer, in any case.

            base.OnExit(e);
        }

        private void InitTimer()
        {
            // Remove old handler in case timer needs to be initiated again.
            if (_onTickEventHandler != null)
                _dispatcherTimer.Tick -= _onTickEventHandler;

            _onTickEventHandler = new EventHandler(OnTick);
            _dispatcherTimer.Tick += _onTickEventHandler;
            _dispatcherTimer.Interval = new TimeSpan(0, (int)Model.Config.TimerIntervalInMinutes, 0);
            _dispatcherTimer.Start();
        }

        private void OnTick(object? sender, EventArgs e)
        {
            if (_notifyIcon != null && _notifyIcon.DataContext != null
                && _notifyIcon.DataContext is ViewModels.NotifyIconViewModel)
            {
                (_notifyIcon.DataContext as ViewModels.NotifyIconViewModel).AddLogCommand.Execute(this);
            }
        }
    }
}
