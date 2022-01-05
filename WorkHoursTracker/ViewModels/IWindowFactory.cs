using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public interface IWindowFactory
    {
        void CreateNewWindow();
        void Show();
    }
}
