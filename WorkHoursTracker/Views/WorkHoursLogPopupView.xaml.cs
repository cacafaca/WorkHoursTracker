using System.Windows;

namespace ProCode.WorkHoursTracker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WorkHoursLogPopupView : Window
    {
        public WorkHoursLogPopupView()
        {
            InitializeComponent();
            
            if(DataContext is ViewModels.WorkHoursLogPopupViewModel)
                ((ViewModels.WorkHoursLogPopupViewModel)DataContext).ConfigWindowFactory = 
                    new BaseWindowFactory(typeof(ConfigView));
        }
    }
}
