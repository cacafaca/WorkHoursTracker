using Microsoft.Win32;
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
        #region Constants
        const string _winRegRunKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        #endregion

        #region Fields
        bool? _startupWithWindows;
        #endregion

        #region Constructors
        public Config()
        {
            // Keep the old settings.
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Reload();

            WorkHoursDirectory = Properties.Settings.Default.WorkHoursDirectory;
            TimerIntervalInMinutes = Properties.Settings.Default.TimerIntervalInMinutes;
            VisibilityIntervalInSeconds = Properties.Settings.Default.VisibilityIntervalInSeconds;

            _startupWithWindows = null;
        }
        #endregion

        #region Properties
        public string WorkHoursDirectory { get; set; }
        public uint TimerIntervalInMinutes { get; set; }
        public uint VisibilityIntervalInSeconds { get; set; }
        public bool StartWithWindowsFlag
        {
            get { return GetStartupFlag(); }
            set { _startupWithWindows = value; }
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
        #endregion

        #region Methods
        public void Save()
        {
            Properties.Settings.Default.WorkHoursDirectory = WorkHoursDirectory;
            Properties.Settings.Default.TimerIntervalInMinutes = TimerIntervalInMinutes;
            Properties.Settings.Default.VisibilityIntervalInSeconds = VisibilityIntervalInSeconds;

            RegistryKey startUpRegKey = Registry.CurrentUser.OpenSubKey(_winRegRunKey, true);
            if ((bool)_startupWithWindows)
                startUpRegKey.SetValue(GetAppName(), System.Environment.ProcessPath);
            else
                startUpRegKey.DeleteValue(GetAppName());
            startUpRegKey.Close();

            Properties.Settings.Default.Save();
        }
        public string GetFileName(DateTime date)
        {
            return $"Report {Environment.UserDomainName}_{Environment.UserName} {date.Year + date.Month.ToString("00")}.xlsx";
        }
        public string GetWorkHoursFilePath(DateTime date)
        {
            return Path.Combine(WorkHoursDirectory, GetFileName(date));
        }
        public string GetAppName()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        }
        private bool GetStartupFlag()
        {
            if (_startupWithWindows == null)
            {
                RegistryKey startUpRegKey = Registry.CurrentUser.OpenSubKey(_winRegRunKey, false);
                if (startUpRegKey != null)
                {
                    object keyData = startUpRegKey.GetValue(GetAppName(), string.Empty);
                    if (keyData != null)
                        _startupWithWindows = !string.IsNullOrEmpty(keyData.ToString());
                    else
                        _startupWithWindows = false;    // Maybe this isn't possible.
                }
                startUpRegKey.Close();
            }
            return (bool)_startupWithWindows;
        }
        #endregion
    }
}
