using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker
{
    public static class Trace
    {
        public static void WriteLine(string message)
        {
            System.Diagnostics.Trace.WriteLine($"WorkHoursTracker> {message}");
        }
    }
}
