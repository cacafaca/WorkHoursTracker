﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProCode.WorkHoursTracker.Views
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class ConfigView : Window
    {
        public ConfigView()
        {
            InitializeComponent();
            if(DataContext is ViewModels.ConfigViewModel viewModel)
                viewModel.DefaultWindowFactory = new BaseWindowFactory(this);
        }
    }
}
