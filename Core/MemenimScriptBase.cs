using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Memenim.Scripting.Core
{
    [RequiredClientVersion("0.15.10", null)]
    public abstract class MemenimScriptBase
    {
        private readonly ReadOnlyDictionary<string, string> _settingsMap;

        public string OriginalName { get; }
        public string Name
        {
            get
            {
                return GetName() ?? string.Empty;
            }
        }
        public string OriginalDescription { get; }
        public string Description
        {
            get
            {
                return GetDescription() ?? string.Empty;
            }
        }
        public string Company { get; }
        public ReadOnlyCollection<string> Authors { get; }
        public ReadOnlyDictionary<string, MemenimScriptCommand> Commands { get; }
        public ReadOnlyDictionary<string, MemenimScriptRuntimeSettingCategory> RuntimeSettingsCategories { get; }
        public ReadOnlyDictionary<string, ReadOnlyDictionary<string, MemenimScriptRuntimeSetting>> RuntimeSettings { get; }

        protected MemenimScriptBase(string name = null,
            string description = null, string company = null,
            IList<string> authors = null)
        {
            _settingsMap = GetSettingsMap();

            OriginalName = NormalizeName(name);
            OriginalDescription = NormalizeDescription(description);
            Company = NormalizeCompany(company);
            Authors = NormalizeAuthors(authors);

            Commands = GetRegisteredCommands();
            (RuntimeSettingsCategories, RuntimeSettings) = GetRegisteredRuntimeSettings();
        }



        internal static string GetBaseLocalizationKey()
        {
            return "[MemenimScript]_script";
        }

        private static string GetNameLocalizationKey()
        {
            var baseLocalizationKey =
                GetBaseLocalizationKey();

            return $"{baseLocalizationKey}-name";
        }

        private static string GetDescriptionLocalizationKey()
        {
            var baseLocalizationKey =
                GetBaseLocalizationKey();

            return $"{baseLocalizationKey}-description";
        }



        public static string GetLocalizationKey(string name)
        {
            var baseLocalizationKey =
                GetBaseLocalizationKey();

            return $"{baseLocalizationKey}_[{name}]";
        }

        public static string GetLocalized(string name)
        {
            var key =
                GetLocalizationKey(name);

            return MemenimScript.Localization
                .GetLocalized(key);
        }
        public static TOut GetLocalized<TOut>(string name)
        {
            var key =
                GetLocalizationKey(name);

            return MemenimScript.Localization
                .GetLocalized<TOut>(key);
        }

        public static string TryGetLocalized(string name)
        {
            var key =
                GetLocalizationKey(name);

            return MemenimScript.Localization
                .TryGetLocalized(key);
        }
        public static TOut TryGetLocalized<TOut>(string name)
        {
            var key =
                GetLocalizationKey(name);

            return MemenimScript.Localization
                .TryGetLocalized<TOut>(key);
        }



        private static string NormalizeName(string name)
        {
            var normalizedName = name;

            if (normalizedName == null)
                return "Unknown";

            normalizedName = normalizedName
                .Trim(' ');

            if (string.IsNullOrEmpty(normalizedName))
                return "Unknown";

            return normalizedName;
        }

        private static string NormalizeDescription(string description)
        {
            var normalizedDescription = description;

            if (normalizedDescription == null)
                return string.Empty;

            normalizedDescription = normalizedDescription
                .Trim(' ');

            if (string.IsNullOrEmpty(normalizedDescription))
                return string.Empty;

            return normalizedDescription;
        }

        private static string NormalizeCompany(string company)
        {
            var normalizeCompany = company;

            if (normalizeCompany == null)
                return "Unknown";

            normalizeCompany = normalizeCompany
                .Trim(' ');

            if (string.IsNullOrEmpty(normalizeCompany))
                return "Unknown";

            return normalizeCompany;
        }

        private static ReadOnlyCollection<string> NormalizeAuthors(IList<string> authors)
        {
            var normalizedAuthors = new List<string>(authors.Count);

            for (int i = 0; i < authors.Count; ++i)
            {
                var author = authors[i];

                if (author == null)
                    continue;

                author = author
                    .Trim(' ');

                if (!string.IsNullOrEmpty(author))
                    normalizedAuthors.Add(author);
            }

            if (normalizedAuthors.Count == 0)
                normalizedAuthors.Add("Unknown");

            return new ReadOnlyCollection<string>(
                normalizedAuthors);
        }



        private string GetName()
        {
            if (!MemenimScript.Localization.IsImplemented)
                return OriginalName;

            var localizedName = MemenimScript.Localization
                .TryGetLocalized(GetNameLocalizationKey());

            if (!string.IsNullOrWhiteSpace(localizedName))
                return localizedName;

            return OriginalName;
        }

        private string GetDescription()
        {
            if (!MemenimScript.Localization.IsImplemented)
                return OriginalDescription;

            var localizedDescription = MemenimScript.Localization
                .TryGetLocalized(GetDescriptionLocalizationKey());

            if (!string.IsNullOrWhiteSpace(localizedDescription))
                return localizedDescription;

            return OriginalDescription;
        }

        private ReadOnlyDictionary<string, string> GetSettingsMap()
        {
            const BindingFlags bindingFlags = BindingFlags.Instance
                                              | BindingFlags.Public
                                              | BindingFlags.NonPublic;

            var settingsMap = new Dictionary<string, string>();

            foreach (var propertyInfo in GetType().GetProperties(bindingFlags))
            {
                if (!Attribute.IsDefined(propertyInfo, typeof(MemenimScriptSettingAttribute)))
                    continue;

                var settingAttribute = (MemenimScriptSettingAttribute)propertyInfo
                    .GetCustomAttribute(typeof(MemenimScriptSettingAttribute));

                var settingName = settingAttribute?.Name;

                if (string.IsNullOrWhiteSpace(settingName))
                    settingName = propertyInfo.Name;

                settingsMap.Add(propertyInfo.Name, settingName);
            }

            return new ReadOnlyDictionary<string, string>(
                settingsMap);
        }

        private ReadOnlyDictionary<string, MemenimScriptCommand> GetRegisteredCommands()
        {
            var commands = new Dictionary<string, MemenimScriptCommand>();

            foreach (var methodInfo in GetType().GetMethods())
            {
                if (!Attribute.IsDefined(methodInfo, typeof(MemenimScriptCommandAttribute)))
                    continue;

                var commandAttribute = (MemenimScriptCommandAttribute)methodInfo
                    .GetCustomAttribute(typeof(MemenimScriptCommandAttribute));

                var commandName = commandAttribute?.Name;
                var commandDescription = commandAttribute?.Description;

                if (string.IsNullOrWhiteSpace(commandName))
                    commandName = methodInfo.Name;
                if (string.IsNullOrWhiteSpace(commandDescription))
                    commandDescription = string.Empty;

                commands.Add(commandName, new MemenimScriptCommand(
                    this, methodInfo, commandName, commandDescription));
            }

            return new ReadOnlyDictionary<string, MemenimScriptCommand>(
                commands);
        }

        private (ReadOnlyDictionary<string, MemenimScriptRuntimeSettingCategory> RuntimeSettingsCategories,
            ReadOnlyDictionary<string, ReadOnlyDictionary<string, MemenimScriptRuntimeSetting>> RuntimeSettings)
            GetRegisteredRuntimeSettings()
        {
            const BindingFlags bindingFlags = BindingFlags.Instance
                                              | BindingFlags.Public
                                              | BindingFlags.NonPublic;

            var runtimeSettingsCategories = new Dictionary<string, MemenimScriptRuntimeSettingCategory>();
            var runtimeSettings = new Dictionary<string, Dictionary<string, MemenimScriptRuntimeSetting>>();

            string runtimeSettingCategoryName = null;

            foreach (var propertyInfo in GetType().GetProperties(bindingFlags))
            {
                if (Attribute.IsDefined(propertyInfo, typeof(MemenimScriptRuntimeSettingCategoryAttribute)))
                {
                    var runtimeSettingCategoryAttribute = (MemenimScriptRuntimeSettingCategoryAttribute)propertyInfo
                        .GetCustomAttribute(typeof(MemenimScriptRuntimeSettingCategoryAttribute));

                    var runtimeSettingCategory = new MemenimScriptRuntimeSettingCategory(
                        !string.IsNullOrWhiteSpace(runtimeSettingCategoryAttribute?.Name)
                            ? runtimeSettingCategoryAttribute.Name
                            : "General");

                    if (!string.IsNullOrEmpty(runtimeSettingCategory.OriginalName)
                        && runtimeSettingCategory.OriginalName != "Unknown")
                    {
                        runtimeSettingCategoryName = runtimeSettingCategory.OriginalName;

                        if (!runtimeSettingsCategories.ContainsKey(runtimeSettingCategoryName))
                        {
                            runtimeSettingsCategories.Add(runtimeSettingCategoryName,
                                runtimeSettingCategory);
                            runtimeSettings.Add(runtimeSettingCategoryName,
                                new Dictionary<string, MemenimScriptRuntimeSetting>());
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(runtimeSettingCategoryName))
                {
                    var runtimeSettingCategory = new MemenimScriptRuntimeSettingCategory(
                        "General");

                    runtimeSettingCategoryName = runtimeSettingCategory.OriginalName;

                    if (!runtimeSettingsCategories.ContainsKey(runtimeSettingCategoryName))
                    {
                        runtimeSettingsCategories.Add(runtimeSettingCategoryName,
                            runtimeSettingCategory);
                        runtimeSettings.Add(runtimeSettingCategoryName,
                            new Dictionary<string, MemenimScriptRuntimeSetting>());
                    }
                }

                if (!Attribute.IsDefined(propertyInfo, typeof(MemenimScriptRuntimeSettingAttribute)))
                    continue;

                var runtimeSettingAttribute = (MemenimScriptRuntimeSettingAttribute)propertyInfo
                    .GetCustomAttribute(typeof(MemenimScriptRuntimeSettingAttribute));

                var runtimeSettingName = runtimeSettingAttribute?.Name;
                var runtimeSettingDescription = runtimeSettingAttribute?.Description;

                if (string.IsNullOrWhiteSpace(runtimeSettingName))
                    runtimeSettingName = propertyInfo.Name;
                if (string.IsNullOrWhiteSpace(runtimeSettingDescription))
                    runtimeSettingDescription = string.Empty;

                var runtimeSetting = new MemenimScriptRuntimeSetting(
                    this, propertyInfo, runtimeSettingName, runtimeSettingDescription);

                if (runtimeSettings[runtimeSettingCategoryName ?? "General"].ContainsKey(runtimeSetting.OriginalName))
                    continue;

                runtimeSettings[runtimeSettingCategoryName ?? "General"].Add(
                    runtimeSetting.OriginalName, runtimeSetting);
            }

            var runtimeSettingsCategoriesReadOnly =
                new ReadOnlyDictionary<string, MemenimScriptRuntimeSettingCategory>(
                    runtimeSettingsCategories);
            var runtimeSettingsReadOnly =
                new ReadOnlyDictionary<string, ReadOnlyDictionary<string, MemenimScriptRuntimeSetting>>(
                    runtimeSettings
                        .Select(runtimeSettingsPair =>
                            new KeyValuePair<string, ReadOnlyDictionary<string, MemenimScriptRuntimeSetting>>(
                                runtimeSettingsPair.Key, new ReadOnlyDictionary<string, MemenimScriptRuntimeSetting>(runtimeSettingsPair.Value)))
                        .ToDictionary(runtimeSettingsPair => runtimeSettingsPair.Key,
                            runtimeSettingsPair => runtimeSettingsPair.Value));

            return (runtimeSettingsCategoriesReadOnly, runtimeSettingsReadOnly);
        }



        public bool ClientVersionSatisfied(ClientVersion version)
        {
            var type = GetType();

            if (!Attribute.IsDefined(type, typeof(RequiredClientVersionAttribute)))
                type = typeof(MemenimScriptBase);

            var requiredVersionAttribute = (RequiredClientVersionAttribute)type
                .GetCustomAttribute(typeof(RequiredClientVersionAttribute));

            if (requiredVersionAttribute == null)
                return true;

            return requiredVersionAttribute.Range
                .IsSatisfied(version);
        }



        protected T GetSetting<T>(
            [CallerMemberName] string propertyName = "")
        {
            var settingName = _settingsMap[propertyName];

            return MemenimScript.Settings
                .Get<T>(settingName);
        }

        protected void SetSetting<T>(T value,
            [CallerMemberName] string propertyName = "")
        {
            var settingName = _settingsMap[propertyName];

            MemenimScript.Settings
                .Set<T>(settingName, value);

            MemenimScript.Settings.Save();
        }
    }
}
