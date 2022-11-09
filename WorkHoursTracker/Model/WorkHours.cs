using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.Model
{
    public class WorkHours
    {
        public WorkHours()
        {
            _task = string.Empty;
            _log = string.Empty;
        }
        private DateOnly _date;

        public DateOnly Date
        {
            get { return _date; }
            set { _date = value; }
        }

        private TimeOnly? _time1In;

        public TimeOnly? Time1In
        {
            get { return _time1In; }
            set { _time1In = value; }
        }

        private TimeOnly? _time1Out;

        public TimeOnly? Time1Out
        {
            get { return _time1Out; }
            set { _time1Out = value; }
        }

        private TimeOnly? _time2In;

        public TimeOnly? Time2In
        {
            get { return _time2In; }
            set { _time2In = value; }
        }

        private TimeOnly _time2Out;

        public TimeOnly Time2Out
        {
            get { return _time2Out; }
            set { _time2Out = value; }
        }

        private string _task;

        public string Task
        {
            get { return _task; }
            set { _task = value; }
        }

        private string _log;

        public string Log
        {
            get { return _log; }
            set { _log = value; }
        }

        public override string ToString()
        {
            var task = !string.IsNullOrWhiteSpace(_task) ? "Task=" + _task : string.Empty;
            var log = !string.IsNullOrWhiteSpace(_log) ? "Log=" + _log : string.Empty;
            var time1 = _time1In != null && _time1Out != null ? string.Join('-', _time1In, _time1Out) : string.Empty;
            return $"{_date}{" " + time1}>{string.Join(' ', task, log)}";
        }
    }
}
