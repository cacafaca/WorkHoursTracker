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

            Left = Screen.PrimaryScreen.WorkingArea.Width - Width - 10;
            Top = Screen.PrimaryScreen.WorkingArea.Height - Height - 10;

            if (DataContext is ViewModels.AddLogViewModel)
            {
                // Send a reference of type of config window. It can be open from a button.
                ((ViewModels.AddLogViewModel)DataContext).ConfigWindowFactory = new BaseWindowFactory(typeof(ConfigView));

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
            if (_moveCursorToEnd)
            {
                _moveCursorToEnd = false;
                logTextBox.Select(logTextBox.Text.Length, 0);       // Position cursor at the end.                
            }
        }
        #endregion
    }
}