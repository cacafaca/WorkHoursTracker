using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class NotifyIconViewModel : BaseViewModel
    {
        public ICommand AddLogCommand { get; set; }
        public ICommand ConfigCommand { get; set; }
        public ICommand ExitApplicationCommand { get; set; }

        public NotifyIconViewModel()
        {
            AddLogCommand = new RelayCommand(AddLogExecute, new Func<object, bool>((obj) => true));
            ConfigCommand = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
            ExitApplicationCommand = new RelayCommand(ExitApplicationExecute, new Func<object, bool>((obj) => true));
        }

        private void ExitApplicationExecute(object obj)
        {
            throw new NotImplementedException();
        }

        private void ConfigExecute(object obj)
        {
            throw new NotImplementedException();
        }

        private void AddLogExecute(object obj)
        {
            if (AddLogWindowFactory != null)
            {

            }
        }

        public IWindowFactory AddLogWindowFactory { get; set; }
    }
}
