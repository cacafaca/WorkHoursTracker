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
        #region Fields
        bool _shown;
        #endregion

        #region Constructors
        public AddLogView()
        {
            InitializeComponent();

            if (DataContext is ViewModels.AddLogViewModel)
            {
                // Send a reference of type of itself.
                ((ViewModels.AddLogViewModel)DataContext).DefaultWindowFactory = new AddLogWindowFactory(this);

                // Send a reference of type of config window. It can be open from a button.
                ((ViewModels.AddLogViewModel)DataContext).ConfigWindowFactory = new BaseWindowFactory(typeof(ConfigView));
            }
        }
        #endregion

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;

            _shown = true;

            // Your code here.
            Left = Screen.PrimaryScreen.WorkingArea.Width - Width - 10;
            Top = Screen.PrimaryScreen.WorkingArea.Height - Height - 10;
        }
    }
}
