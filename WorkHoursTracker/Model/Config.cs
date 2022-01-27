using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.Model
{
    public static class Config
    {
        #region Constants
        const string _winRegRunKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        #endregion

        #region Fields
        static bool? _startWithWindows;
        #endregion

        #region Constructors
        static Config()
        {
            ReadSettingsFromRegistry();
            _startWithWindows = null;
        }
        #endregion

        #region Delegates
        public delegate void ConfigSavedHandler(EventArgs e);
        #endregion

        #region Events
        public static event ConfigSavedHandler ConfigSaved;
        #endregion

        #region Properties
        public static string WorkHoursDirectory { get; set; }
        public static uint TimerIntervalInMinutes { get; set; }
        public static uint VisibilityIntervalInSeconds { get; set; }
        public static bool StartWithWindowsFlag
        {
            get { return GetStartupFlag(); }
            set { _startWithWindows = value; }
        }
        /// <summary>
        /// Full path of the current file.
        /// </summary>
        public static string WorkHoursCurrentFilePath
        {
            get
            {
                return GetWorkHoursFilePath(DateTime.Now);
            }
        }
        #endregion

        #region Methods
        public static void Save()
        {
            // Start with Windows.
            RegistryKey startUpRegKey = Registry.CurrentUser.OpenSubKey(_winRegRunKey, true);
            if (startUpRegKey != null)
            {
                if (_startWithWindows != null && (bool)_startWithWindows)
                    startUpRegKey.SetValue(GetAppName(), Environment.ProcessPath);
                else
                {
                    if (startUpRegKey.GetValue(GetAppName()) != null)
                        startUpRegKey.DeleteValue(GetAppName());
                }
                startUpRegKey.Close();
            }

            // Application data.
            RegistryKey appRegistrySettings = Registry.CurrentUser.OpenSubKey(GetAppRegistryPath(), true);
            if (appRegistrySettings != null)
            {
                appRegistrySettings.SetValue(Properties.Settings.Default.WorkHoursDirectoryRegistryName, WorkHoursDirectory);
                appRegistrySettings.SetValue(Properties.Settings.Default.TimerIntervalInMinutesRegistryName, TimerIntervalInMinutes, RegistryValueKind.DWord);
                appRegistrySettings.SetValue(Properties.Settings.Default.VisibilityIntervalInSecondsRegistryName, VisibilityIntervalInSeconds, RegistryValueKind.DWord);
                appRegistrySettings.Close();
            }
            ConfigSaved?.Invoke(new EventArgs());
        }

        /// <summary>
        /// Returns file name with a certain pattern.
        /// </summary>
        /// <param name="date">Determines year and month.</param>
        /// <returns></returns>
        public static string GetFileName(DateTime date)
        {
            return $"Report {Environment.UserDomainName}_{Environment.UserName} {date.Year + date.Month.ToString("00")}.xlsx";
        }
        public static string GetWorkHoursFilePath(DateTime date)
        {
            return Path.Combine(WorkHoursDirectory, GetFileName(date));
        }
        public static string GetAppName()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        }
        private static bool GetStartupFlag()
        {
            if (_startWithWindows == null)
            {
                RegistryKey startUpRegKey = Registry.CurrentUser.OpenSubKey(_winRegRunKey, false);
                if (startUpRegKey != null)
                {
                    object keyData = startUpRegKey.GetValue(GetAppName());
                    _startWithWindows = keyData != null ? File.Exists(keyData.ToString()) : false;
                }
                startUpRegKey.Close();
            }
            return (bool)_startWithWindows;
        }
        private static string GetAppRegistryPath()
        {
            return $"SOFTWARE\\ProCode\\{GetAppName()}";
        }
        private static void ReadSettingsFromRegistry()
        {
            RegistryKey appRegistrySettings = Registry.CurrentUser.OpenSubKey(GetAppRegistryPath(), false);
            if (appRegistrySettings != null)
            {
                WorkHoursDirectory = appRegistrySettings.GetValue(Properties.Settings.Default.WorkHoursDirectoryRegistryName, Properties.Settings.Default.WorkHoursDirectoryDefaultValue).ToString();
                TimerIntervalInMinutes = Convert.ToUInt32(appRegistrySettings.GetValue(Properties.Settings.Default.TimerIntervalInMinutesRegistryName, Properties.Settings.Default.TimerIntervalInMinutesDefaultValue));
                VisibilityIntervalInSeconds = Convert.ToUInt32(appRegistrySettings.GetValue(Properties.Settings.Default.VisibilityIntervalInSecondsRegistryName, Properties.Settings.Default.VisibilityIntervalInSecondsDefaultValue));
                appRegistrySettings.Close();
            }
            else
            {
                appRegistrySettings = Registry.CurrentUser.CreateSubKey(GetAppRegistryPath(), true);
                appRegistrySettings.SetValue(Properties.Settings.Default.WorkHoursDirectoryRegistryName, Properties.Settings.Default.WorkHoursDirectoryDefaultValue);
                appRegistrySettings.SetValue(Properties.Settings.Default.TimerIntervalInMinutesRegistryName, Properties.Settings.Default.TimerIntervalInMinutesDefaultValue, RegistryValueKind.DWord);
                appRegistrySettings.SetValue(Properties.Settings.Default.VisibilityIntervalInSecondsRegistryName, Properties.Settings.Default.VisibilityIntervalInSecondsDefaultValue, RegistryValueKind.DWord);
                appRegistrySettings.Close();

                WorkHoursDirectory = Properties.Settings.Default.WorkHoursDirectoryDefaultValue;
                TimerIntervalInMinutes = Properties.Settings.Default.TimerIntervalInMinutesDefaultValue;
                VisibilityIntervalInSeconds = Properties.Settings.Default.VisibilityIntervalInSecondsDefaultValue;
            }
        }
        #endregion
    }
}
