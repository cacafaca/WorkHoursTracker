namespace ProCode.WorkHoursTracker.ViewModels
{
    public interface IWindowFactory
    {
        void CreateWindow(object? creator = null);
        void ShowWindow();
        void CloseWindow();
        bool IsVisible();
        bool IsCreated();
    }
}
