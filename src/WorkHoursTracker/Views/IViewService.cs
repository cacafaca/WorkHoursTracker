namespace ProCode.WorkHoursTracker.Views
{
    public interface IViewService
    {
        enum WindowType
        {
            AddLogText,
            AddLogTable,
            Config
        }
        public void Open(WindowType windowType, object? dataContext = null);
        public void Close();
        public bool IsLoaded();
        public bool IsVisible();
    }
}