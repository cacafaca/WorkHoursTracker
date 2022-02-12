using System;
using System.Windows;
using System.Windows.Forms;

namespace ProCode.WorkHoursTracker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AddLogView : Window
    {
        #region Feilds
        bool _moveCursorToEnd = true;
        #endregion

        #region Constructors
        public AddLogView()
        {
            InitializeComponent();

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
                ((ViewModels.AddLogViewModel)DataContext).Closing += AddLogView_Closing;
            }
        }

        #endregion

        #region Methods
        private void AddLogView_Closing()
        {
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
            ConfigView cfgView = new ConfigView();
            cfgView.ShowDialog();
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
        #endregion

    }
}