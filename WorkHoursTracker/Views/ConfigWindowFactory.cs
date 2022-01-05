using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.Views
{
    public class ConfigWindowFactory : ViewModels.IWindowFactory
    {
        ConfigView _configWindow;

        public void CreateNewWindow()
        {
            _configWindow = new ConfigView
            {
                DataContext = new ViewModels.ConfigViewModel()
            };
        }

        public void Show()
        {
            if(_configWindow != null)
                _configWindow.Show();
        }
    }
}
