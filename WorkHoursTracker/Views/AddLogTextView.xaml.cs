using System;
using System.Windows;
using System.Windows.Forms;

namespace ProCode.WorkHoursTracker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AddLogTextView : Window
    {
        #region Fields
        bool _moveCursorToEnd = true;
        #endregion

        #region Constructors
        public AddLogTextView(object dataContext)
        {
            if (dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            InitializeComponent();

            DataContext = dataContext;

            var dpi = System.Windows.Media.VisualTreeHelper.GetDpi(this);
            Trace.WriteLine($"PrimaryScreen.WorkingArea (Width, Height) = ({Screen.PrimaryScreen.WorkingArea.Width}, {Screen.PrimaryScreen.WorkingArea.Height}).");
            Trace.WriteLine($"AddLog (Width, Height) = ({Width}, {Height}).");
            Left = Screen.PrimaryScreen.WorkingArea.Width / dpi.DpiScaleX - Width - 10;     // DPI Awareness is not working!
            if (Left < 0) Left = 0;
            Top = Screen.PrimaryScreen.WorkingArea.Height / dpi.DpiScaleY - Height - 10;
            if (Top < 0) Top = 0;
            Trace.WriteLine($"AddLog (Left, Top) = ({Left}, {Top}).");

            if (DataContext is ViewModels.AddLogViewModel)
            {
                ((ViewModels.AddLogViewModel)DataContext).AfterPaste += AddLogTextView_AfterPaste;
            }
        }

        public AddLogTextView(IViewService viewService) : this(new ViewModels.AddLogViewModel(viewService)) { }
        #endregion

        #region Methods
        private void AddLogTextView_Closing()
        {
            ((ViewModels.AddLogViewModel)DataContext).AfterPaste -= AddLogTextView_AfterPaste;
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();     // Move window around the screen.
        }

        private void logTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_moveCursorToEnd)                                   // Position cursor at the end only one time, because user wants to start typing immediately. 
            {
                _moveCursorToEnd = false;
                logTextBox.Select(logTextBox.Text.Length, 0);       // Position cursor at the end.                
            }
        }

        private void configButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Topmost = true;
        }

        private void logTextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (logTextBox.Visibility == Visibility.Visible)
                logTextBox.Focus();
        }

        private void AddLogTextView_AfterPaste(object? sender, EventArgs e)
        {
            logTextBox.Focus();
            logTextBox.Select(logTextBox.Text.Length, 0);
        }
        #endregion

    }
}