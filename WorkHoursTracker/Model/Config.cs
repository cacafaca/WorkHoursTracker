using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        /// <summary>
        /// Triggered when configuration is saved.
        /// </summary>
        public static event ConfigSavedHandler ConfigSaved;
        #endregion

        #region Properties
        [RegistryValue("Please enter work hours directory...")]
        [Description("Work hours directory")]
        public static string WorkHoursDirectory { get; set; }
        [RegistryValue(45)]
        [Description("Remind interval (minutes)")]
        public static int TimerIntervalInMinutes { get; set; }
        [RegistryValue("09:00")]
        [Description("Don't remind before")]
        public static string WorkHourStart { get; set; }
        [RegistryValue("17:00")]
        [Description("Don't remind after")]
        public static string WorkHourEnd { get; set; }
        public static bool StartWithWindowsFlag
        {
            get { return GetStartupFlag(); }
            set { _startWithWindows = value; }
        }
        /// <summary>
        /// Full path of the file for the current month.
        /// </summary>
        public static string WorkHoursCurrentFilePath
        {
            get
            {
                return GetWorkHoursFilePath(DateTime.Now);
            }
        }

        /// <summary>
        /// Full path of the file for the previous month.
        /// </summary>
        public static string WorkHoursPreviousFilePath
        {
            get
            {
                return GetWorkHoursFilePath(DateTime.Today.AddMonths(-1));
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
                foreach (var regProp in GetRegistryProperties())
                {
                    appRegistrySettings.SetValue(regProp.Name, regProp.Value);
                }
                appRegistrySettings.Close();
            }
            ConfigSaved?.Invoke(new EventArgs());
        }
        public static List<RegistryProperty> GetRegistryProperties()
        {
            return typeof(Config).GetProperties()
                .Where(p => p.CustomAttributes.Any(ca => ca.AttributeType.FullName == typeof(RegistryValueAttribute).FullName))
                .Select(propInfo => new RegistryProperty
                {
                    Name = propInfo.Name,
                    Description = ((DescriptionAttribute)Attribute.GetCustomAttribute(propInfo, typeof(DescriptionAttribute))).Description,
                    Value = propInfo.GetValue(propInfo.Name)
                }).ToList();
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
        internal static void SetRegistryProperties(List<RegistryProperty> registryProperties)
        {
            foreach (var regValue in registryProperties)
            {
                SetRegistryProperty(regValue);
            }
        }
        internal static void SetRegistryProperty(RegistryProperty registryProperty)
        {
            var prop = typeof(Config).GetProperty(registryProperty.Name);
            if (prop != null)
            {
                switch(Type.GetTypeCode(prop.PropertyType))
                {
                    case TypeCode.String:
                        prop.SetValue(prop, registryProperty.Value);
                        break;
                    case TypeCode.Int32:
                        prop.SetValue(prop, Convert.ToInt32(registryProperty.Value));
                        break;
                }
                
            }
                
        }

        private static bool GetStartupFlag()
        {
            if (_startWithWindows == null)
            {
                RegistryKey? startUpRegKey = Registry.CurrentUser.OpenSubKey(_winRegRunKey, false);
                if (startUpRegKey != null)
                {
                    object? executablePath = startUpRegKey.GetValue(GetAppName());
                    _startWithWindows = executablePath != null ? File.Exists(executablePath.ToString()) : false;
                    startUpRegKey.Close();
                }
                else
                {
                    _startWithWindows = false;
                }
            }
            return (bool)_startWithWindows;
        }
        private static string GetAppRegistryPath()
        {
            return $"SOFTWARE\\ProCode\\{GetAppName()}";
        }
        private static void ReadSettingsFromRegistry()
        {
            RegistryKey appRegistrySettings = Registry.CurrentUser.CreateSubKey(GetAppRegistryPath(), true);
            if (appRegistrySettings != null)
            {
                foreach (var regProp in GetRegistryProperties())
                {
                    var prop = typeof(Config).GetProperties().FirstOrDefault(p => p.Name == regProp.Name);
                    if (prop != null)
                    {
                        prop.SetValue(prop, appRegistrySettings.GetValue(prop.Name, ((RegistryValueAttribute)Attribute.GetCustomAttribute(prop, typeof(RegistryValueAttribute))).DefailtValue));
                    }
                }
                appRegistrySettings.Close();
            }
        }
        #endregion
    }

    /// <summary>
    /// Decorates property as regisrty value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RegistryValueAttribute : Attribute
    {
        #region Constructor
        public RegistryValueAttribute(object defaultValue)
        {
            DefailtValue = defaultValue;
        }
        #endregion

        #region Properties
        public object DefailtValue { get; private set; }
        #endregion
    }

    public class RegistryProperty
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public object Value { get; set; }
    }
}
