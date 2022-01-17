using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows;

namespace Procode.WorkHoursTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon;
        private System.Windows.Threading.DispatcherTimer _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            if (_notifyIcon != null && _notifyIcon.DataContext != null &&
                _notifyIcon.DataContext is ProCode.WorkHoursTracker.ViewModels.NotifyIconViewModel)
            {
                // Set Log window type.
                ((ProCode.WorkHoursTracker.ViewModels.NotifyIconViewModel)_notifyIcon.DataContext).AddLogWindowFactory =
                    new ProCode.WorkHoursTracker.Views.AddLogWindowFactory(typeof(ProCode.WorkHoursTracker.Views.AddLogView));

                // Set Config window type.
                ((ProCode.WorkHoursTracker.ViewModels.NotifyIconViewModel)_notifyIcon.DataContext).ConfigWindowFactory =
                    new ProCode.WorkHoursTracker.Views.BaseWindowFactory(typeof(ProCode.WorkHoursTracker.Views.ConfigView));
                
                InitTimer();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            _dispatcherTimer.Stop();

            base.OnExit(e);
        }

        private void InitTimer()
        {
            _dispatcherTimer.Tick += new EventHandler(OnTick);
            _dispatcherTimer.Interval = new TimeSpan(0, (int)ProCode.WorkHoursTracker.Properties.Settings.Default.TimerIntervalInMinutes, 0);
            _dispatcherTimer.Start();
        }

        private void OnTick(object? sender, EventArgs e)
        {
            if (_notifyIcon != null && _notifyIcon.DataContext != null &&
                _notifyIcon.DataContext is ProCode.WorkHoursTracker.ViewModels.NotifyIconViewModel)
            {
                (_notifyIcon.DataContext as ProCode.WorkHoursTracker.ViewModels.NotifyIconViewModel).AddLogCommand.Execute(this);
            }
        }
    }
}
