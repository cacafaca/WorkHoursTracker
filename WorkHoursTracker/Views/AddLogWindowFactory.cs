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
            // I wanted something special to do here, but turns out that I don't need it. So I leave it as is.
            base.ShowWindow();
        }

        public override void CreateWindow(object? creator = null)
        {
            base.CreateWindow(creator);
            _window.ShowActivated = false;  // Prevent stealing focus from other apps.

            if (creator is App && _window.DataContext is ViewModels.AddLogViewModel)
            {
                ((ViewModels.AddLogViewModel)_window.DataContext).StartCloseTimerCommand.Execute(this);
            }
        }
    }
}
