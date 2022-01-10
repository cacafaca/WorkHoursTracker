using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace Procode.WorkHoursTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            if (_notifyIcon != null && _notifyIcon.DataContext != null &&
                _notifyIcon.DataContext is ProCode.WorkHoursTracker.ViewModels.NotifyIconViewModel)
            {
                // Set Log window type.
                ((ProCode.WorkHoursTracker.ViewModels.NotifyIconViewModel)_notifyIcon.DataContext).WorkHoursLogPopupWindowFactory =
                    new ProCode.WorkHoursTracker.Views.BaseWindowFactory(typeof(ProCode.WorkHoursTracker.Views.AddLogView));

                // Set Config window type.
                ((ProCode.WorkHoursTracker.ViewModels.NotifyIconViewModel)_notifyIcon.DataContext).ConfigWindowFactory =
                    new ProCode.WorkHoursTracker.Views.BaseWindowFactory(typeof(ProCode.WorkHoursTracker.Views.ConfigView));
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}
