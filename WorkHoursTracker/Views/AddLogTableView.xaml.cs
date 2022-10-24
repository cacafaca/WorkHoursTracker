using System;
using System.Windows;
using System.Windows.Forms;

namespace ProCode.WorkHoursTracker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AddLogTableView : Window
    {
        #region Fields
        #endregion

        #region Constructors
        public AddLogTableView(object dataContext)
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
        }

        public AddLogTableView(IViewService viewService) : this(new ViewModels.AddLogViewModel(viewService)) { }
        #endregion

        #region Methods
        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();     // Move window around the screen.
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Topmost = true;
        }
        #endregion

    }
}