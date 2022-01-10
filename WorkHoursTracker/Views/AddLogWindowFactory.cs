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
            _window.Left = Screen.PrimaryScreen.WorkingArea.Width - _window.Width - 10;
            _window.Top = Screen.PrimaryScreen.WorkingArea.Height - _window.Height - 10;

            base.ShowWindow();
        }
    }
}
