using System;
using System.Globalization;
using System.Reflection;

namespace Memenim.Scripting.Core
{
    public sealed class MemenimScriptRuntimeSetting
    {
        private readonly MemenimScriptBase _script;
        private readonly PropertyInfo _property;

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
        public Type Type { get; }
        public object Value
        {
            get
            {
                return GetValue<object>();
            }
            set
            {
                SetValue(value);
            }
        }

        internal MemenimScriptRuntimeSetting(
            MemenimScriptBase script, PropertyInfo property,
            string name = null, string description = null)
        {
            _script = script;
            _property = property;

            OriginalName = NormalizeName(name);
            OriginalDescription = NormalizeDescription(description);

            Type = property.PropertyType;
        }



        internal string GetBaseLocalizationKey()
        {
            var baseLocalizationKey =
                MemenimScriptBase.GetBaseLocalizationKey();

            return $"{baseLocalizationKey}|[{OriginalName}]_runtime+setting+category";
        }

        private string GetNameLocalizationKey()
        {
            var baseLocalizationKey =
                GetBaseLocalizationKey();

            return $"{baseLocalizationKey}-name";
        }

        private string GetDescriptionLocalizationKey()
        {
            var baseLocalizationKey =
                GetBaseLocalizationKey();

            return $"{baseLocalizationKey}-description";
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



        private string GetName()
        {
            if (!MemenimScript.Localization.IsImplemented)
                return OriginalName;

            var key = GetNameLocalizationKey();

            if (MemenimScript.Localization.TryGetLocalized(key, out var localizedName)
                && !string.IsNullOrWhiteSpace(localizedName))
            {
                return localizedName;
            }

            return OriginalName;
        }

        private string GetDescription()
        {
            if (!MemenimScript.Localization.IsImplemented)
                return OriginalDescription;

            var key = GetDescriptionLocalizationKey();

            if (MemenimScript.Localization.TryGetLocalized(key, out var localizedDescription)
                && !string.IsNullOrWhiteSpace(localizedDescription))
            {
                return localizedDescription;
            }

            return OriginalDescription;
        }



        private T GetValue<T>()
        {
            const BindingFlags bindingFlags = BindingFlags.Instance
                                              | BindingFlags.Public
                                              | BindingFlags.NonPublic;

            return (T)_property.GetValue(_script,
                bindingFlags, null, null,
                CultureInfo.InvariantCulture);
        }

        private void SetValue<T>(T value)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance
                                              | BindingFlags.Public
                                              | BindingFlags.NonPublic;

            _property.SetValue(_script, value,
                bindingFlags, null, null,
                CultureInfo.InvariantCulture);
        }
    }
}
