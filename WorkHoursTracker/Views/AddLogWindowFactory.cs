using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace ProCode.WorkHoursTracker.Views
{
    public class AddLogWindowFactory : BaseWindowFactory
    {
        public AddLogWindowFactory(Type windowType) : base(windowType)
        {
        }

        public AddLogWindowFactory(Window window) : base(window)
        {
        }

        public override void ShowWindow()
        {
            _window.ShowActivated = false;  // Prevent stealing focus from other apps.
            _window.Left = Screen.PrimaryScreen.WorkingArea.Width - _window.Width - 10;
            _window.Top = Screen.PrimaryScreen.WorkingArea.Height - _window.Height - 10;
            base.ShowWindow();
            _window.Topmost = true;
            _window.Topmost = false;
        }

        public override void CreateWindow(object? creator = null)
        {
            base.CreateWindow(creator);

            if (creator is App && _window.DataContext is ViewModels.AddLogViewModel)
            {
                ((ViewModels.AddLogViewModel)_window.DataContext).StartCloseTimerCommand.Execute(this);
            }
        }
    }
}
