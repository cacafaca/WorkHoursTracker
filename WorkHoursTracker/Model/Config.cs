using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.Model
{
    public class Config
    {
        public string WorkHoursDirectory { get; set; }
        public uint TimerIntervalInMinutes { get; set; }
        public uint VisibilityIntervalInSeconds { get; set; }

        public Config()
        {
            // Keep the old settings.
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Reload();

            WorkHoursDirectory = Properties.Settings.Default.WorkHoursDirectory;
            TimerIntervalInMinutes = Properties.Settings.Default.TimerIntervalInMinutes;
            VisibilityIntervalInSeconds = Properties.Settings.Default.VisibilityIntervalInSeconds;
        }
        public void Save()
        {
            Properties.Settings.Default.WorkHoursDirectory = WorkHoursDirectory;
            Properties.Settings.Default.TimerIntervalInMinutes = TimerIntervalInMinutes;
            Properties.Settings.Default.VisibilityIntervalInSeconds = VisibilityIntervalInSeconds;

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Full path of the current file.
        /// </summary>
        public string WorkHoursCurrentFilePath
        {
            get
            {
                return GetWorkHoursFilePath(DateTime.Now);
            }
        }

        public string GetFileName(DateTime date)
        {
            return $"Report {Environment.UserDomainName}_{Environment.UserName} {date.Year + date.Month.ToString("00")}.xlsx";
        }

        public string GetWorkHoursFilePath(DateTime date)
        {
            return Path.Combine(WorkHoursDirectory, GetFileName(date));
        }
    }
}
