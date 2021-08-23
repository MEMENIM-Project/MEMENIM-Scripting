using System;

namespace Memenim.Scripting.Core
{
    public sealed class MemenimScriptRuntimeSettingCategory
    {
        public string OriginalName { get; }
        public string Name
        {
            get
            {
                return GetName() ?? string.Empty;
            }
        }

        internal MemenimScriptRuntimeSettingCategory(
            string name = null)
        {
            OriginalName = NormalizeName(name);
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
    }
}
