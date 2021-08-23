using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using RIS.Settings;
using RIS.Settings.Ini;

namespace Memenim.Scripting.Core
{
    public class MemenimScriptSettings : IniSettings
    {
        private const string SettingsFileName = "MemenimScriptSettings.config";

        private readonly ReadOnlyDictionary<string, PropertyInfo> _settings;

        public MemenimScriptSettings()
            : base(Path.Combine(
                Path.GetDirectoryName(typeof(MemenimScriptSettings).Assembly.Location) ?? string.Empty,
                SettingsFileName))
        {
            _settings = GetRegisteredSettings();
        }



        private ReadOnlyDictionary<string, PropertyInfo> GetRegisteredSettings()
        {
            const BindingFlags bindingFlags = BindingFlags.Instance
                                              | BindingFlags.Public;

            var settings = new Dictionary<string, PropertyInfo>(5);

            foreach (var propertyInfo in GetType().GetProperties(bindingFlags))
            {
                if (Attribute.IsDefined(propertyInfo, typeof(ExcludedSettingAttribute)))
                    continue;

                settings.Add(propertyInfo.Name, propertyInfo);
            }

            return new ReadOnlyDictionary<string, PropertyInfo>(
                settings);
        }



#pragma warning disable IDE0060 // Удалите неиспользуемый параметр
        // ReSharper disable UnusedParameter.Global
        public new void Load(
            SettingsLoadOptions options = SettingsLoadOptions.None,
            bool appVersionCheck = false,
            SettingsLoadOptions appVersionCheckOptions = SettingsLoadOptions.None)
        {
            base.Load(options, false,
                SettingsLoadOptions.None);
        }
        // ReSharper enable UnusedParameter.Global
#pragma warning restore IDE0060 // Удалите неиспользуемый параметр

        public new void Save()
        {
            base.Save();
        }



        internal T Get<T>(
            string propertyName)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance
                                              | BindingFlags.Public
                                              | BindingFlags.NonPublic;

            return (T)_settings[propertyName].GetValue(this,
                bindingFlags, null, null,
                CultureInfo.InvariantCulture);
        }

        internal void Set<T>(
            string propertyName, T value)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance
                                              | BindingFlags.Public
                                              | BindingFlags.NonPublic;

            _settings[propertyName].SetValue(this, value,
                bindingFlags, null, null,
                CultureInfo.InvariantCulture);
        }
    }
}
