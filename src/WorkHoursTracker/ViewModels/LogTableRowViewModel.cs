using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.ViewModels
{
    public class LogTableRowViewModel : BaseViewModel
    {
        #region Fields
        private string _task;
        private float _spent;
        #endregion

        #region Constructor
        public LogTableRowViewModel()
        {
            _task = string.Empty;
        }
        #endregion

        #region Properties
        public string Task
        {
            get { return _task; }
            set
            {
                _task = value;
                OnPropertyChanged();
            }
        }
        public float Spent
        {
            get { return _spent; }
            set
            {
                _spent = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{_task}[{_spent}]";
        }
        #endregion

    }
}
