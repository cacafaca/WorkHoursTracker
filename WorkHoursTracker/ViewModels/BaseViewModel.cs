using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Delagates
        public delegate void ClosingHandler();
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event ClosingHandler Closing;
        #endregion

        #region Methods
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected void InvokeClosingEvent()
        {
            Closing?.Invoke();
        }
        #endregion
    }
}
