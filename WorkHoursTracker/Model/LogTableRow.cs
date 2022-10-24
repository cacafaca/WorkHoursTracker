using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.Model
{
    public class LogTableRow
    {
        #region Constructor
        public LogTableRow()
        {
            Task = string.Empty;
        }
        #endregion

        #region Properties
        public string Task { get; set; }
        public float Spent { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{Task}[{Spent}]";
        }
        #endregion
    }
}
