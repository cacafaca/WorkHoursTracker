namespace ProCode.WorkHoursTracker.ViewModels
{
    public interface IWindowFactory
    {
        void CreateWindow(object? creator = null);
        void ShowWindow();
        bool IsVisible();
        bool IsCreated();
    }
}
