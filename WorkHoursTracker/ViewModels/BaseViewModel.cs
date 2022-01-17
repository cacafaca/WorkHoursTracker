using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        
        /// <summary>
        /// This should contain reference to corresponding view. 
        /// </summary>
        public IWindowFactory DefaultWindowFactory { get; set; }
    }
}
