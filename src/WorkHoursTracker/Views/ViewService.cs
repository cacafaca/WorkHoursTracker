using System.Windows;
using static ProCode.WorkHoursTracker.Views.IViewService;

namespace ProCode.WorkHoursTracker.Views
{
    internal class ViewService : IViewService
    {
        #region Fields
        private Window? _activeView;
        #endregion

        #region Methods
        public void Open(WindowType windowType, object? dataContext = null)
        {
            switch (windowType)
            {
                case WindowType.AddLogText:
                    CreateAddLogText(dataContext);
                    break;
                case WindowType.AddLogTable:
                    CreateAddLogTable(dataContext);
                    break;
                case WindowType.Config:
                    _activeView = new ConfigView(this);
                    break;
            }
            _activeView?.Show();
        }

        public void Close()
        {
            _activeView?.Close();
            _activeView = null;
        }

        public bool IsLoaded() => _activeView != null && _activeView.IsLoaded;

        public bool IsVisible() => _activeView != null && _activeView.IsVisible;

        private void CreateAddLogTable(object? dataContext = null)
        { 
            if (dataContext != null)
                _activeView = new AddLogTableView(dataContext);
            else
                _activeView = new AddLogTableView(this);
        }

        private void CreateAddLogText(object? dataContext = null)
        {
            if (dataContext != null)
                _activeView = new AddLogTextView(dataContext);
            else
                _activeView = new AddLogTextView(this);
        }
        #endregion
    }
}
