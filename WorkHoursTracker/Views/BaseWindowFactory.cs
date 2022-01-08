using System;
using System.Windows;

namespace ProCode.WorkHoursTracker.Views
{
    public class BaseWindowFactory : ViewModels.IWindowFactory
    {
        Window _window;
        Type _windowType;

        public BaseWindowFactory(Type windowType)
        {
            if (windowType == null)
                throw new ArgumentNullException(nameof(windowType));

            if (!windowType.IsSubclassOf(typeof(Window)))
                throw new ArgumentException("Expect Wescendant of Window type.", nameof(windowType));

            _windowType = windowType;
        }

        public void CreateWindow()
        {
            _window = (Window)Activator.CreateInstance(_windowType);
        }

        public void ShowWindow()
        {
            _window?.Show();
        }

        public void CloseWindow()
        {
            _window.Close();
            _window = null;
        }
    }
}
