using System;
using System.Windows;

namespace ProCode.WorkHoursTracker.Views
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class ConfigView : Window
    {
        #region Constructors
        public ConfigView(IViewService viewService)
        {
            InitializeComponent();

            if (DataContext == null)
            {
                if (viewService != null)
                    DataContext = new ViewModels.ConfigViewModel(viewService);
                else
                    DataContext = new ViewModels.ConfigViewModel();
            }

            if (DataContext is ViewModels.ConfigViewModel configViewModel)
            {
                foreach (var param in configViewModel.Parameters)
                {
                    param.PickTimeEvent += Param_PickTimeEvent; ;
                }
            }
        }

        #endregion

        #region Methods
        private void Param_PickTimeEvent(ref TimeOnly time)
        {
            MessageBox.Show("Only in paid version.");
        }

        #endregion
    }
}
