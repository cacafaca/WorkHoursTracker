namespace ProCode.WorkHoursTracker.ViewModels
{
    public interface IWindowFactory
    {
        void CreateWindow();
        void ShowWindow();
        void CloseWindow();
        bool IsVisible();
        bool IsCreated();
    }
}
