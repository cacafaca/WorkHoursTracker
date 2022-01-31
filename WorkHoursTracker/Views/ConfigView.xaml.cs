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
        public ConfigView()
        {
            InitializeComponent();
            if (DataContext is ViewModels.ConfigViewModel configViewModel)
            {
                configViewModel.Closing += ViewModel_Closing;
                foreach (var param in configViewModel.Parameters)
                {
                    param.PickTimeEvent += Param_PickTimeEvent; ;
                }
            }
        }

        #endregion

        #region Methods
        private void ViewModel_Closing()
        {
            Close();
        }
        private void Param_PickTimeEvent(ref TimeOnly time)
        {
            MessageBox.Show("Only in paid version.");
        }

        #endregion
    }
}
