using System;
using System.Windows;

namespace ProCode.WorkHoursTracker.Views
{
    public class BaseWindowFactory : ViewModels.IWindowFactory
    {
        #region Fields
        protected Window _window;
        protected Type _windowType;
        #endregion

        #region Constructors
        public BaseWindowFactory(Type windowType)
        {
            if (windowType == null)
                throw new ArgumentNullException(nameof(windowType));

            if (!windowType.IsSubclassOf(typeof(Window)))
                throw new ArgumentException("Expect Wescendant of Window type.", nameof(windowType));

            _windowType = windowType;
        }

        public BaseWindowFactory(Window window)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));

            if (!window.GetType().IsSubclassOf(typeof(Window)))
                throw new ArgumentException("Expect Descendant of Window type.", nameof(window));

            _window = window;
            _windowType = window.GetType();
        }
        #endregion

        public virtual void CreateWindow()
        {
            _window = (Window)Activator.CreateInstance(_windowType);
        }

        public virtual void ShowWindow()
        {
            _window?.Show();
        }

        public virtual void CloseWindow()
        {
            _window?.Close();
            _window = null;
        }

        public bool IsVisible()
        {
            return _window.IsVisible;
        }

        public bool IsCreated()
        {
            return _window != null;
        }
    }
}
