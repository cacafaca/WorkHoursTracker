﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class NotifyIconViewModel : BaseViewModel
    {
        public ICommand AddLogCommand { get; set; }
        public ICommand ConfigCommand { get; set; }
        public ICommand ExitApplicationCommand { get; set; }
        public ICommand OpenExcelCommand { get; set; }

        public NotifyIconViewModel()
        {
            AddLogCommand = new RelayCommand(AddLogExecute, new Func<object, bool>((obj) => true));
            ConfigCommand = new RelayCommand(ConfigExecute, new Func<object, bool>((obj) => true));
            ExitApplicationCommand = new RelayCommand(ExitApplicationExecute, new Func<object, bool>((obj) => true));
            OpenExcelCommand = new RelayCommand(OpenExcelExecute, new Func<object, bool>((obj) => true));
        }

        private void OpenExcelExecute(object obj)
        {
            WorkHoursMonthlyExcel whExcel = new WorkHoursMonthlyExcel(new Model.Config().WorkHoursCurrentFilePath);
            whExcel.Open();
        }

        private void ExitApplicationExecute(object obj)
        {
            if (obj != null && obj is System.Windows.Window)
            {
                ((System.Windows.Window)obj).Close();
            }
        }

        private void ConfigExecute(object obj)
        {
            if (ConfigWindowFactory != null)
            {
                ConfigWindowFactory.CreateWindow();
                ConfigWindowFactory.ShowWindow();
            }
        }

        private void AddLogExecute(object obj)
        {
            if (WorkHoursLogPopupWindowFactory != null)
            {
                WorkHoursLogPopupWindowFactory.CreateWindow();
                WorkHoursLogPopupWindowFactory.ShowWindow();
            }
        }

        public IWindowFactory WorkHoursLogPopupWindowFactory { get; set; }
        public IWindowFactory ConfigWindowFactory { get; set; }
    }
}
