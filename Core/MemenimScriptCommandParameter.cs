using System;

namespace Memenim.Scripting.Core
{
    public sealed class MemenimScriptCommandParameter
    {
        private MemenimScriptCommand Command { get; }

        public string Name { get; }
        public string OriginalDescription { get; }
        public string Description
        {
            get
            {
                return GetDescription() ?? string.Empty;
            }
        }
        public Type Type { get; }

        internal MemenimScriptCommandParameter(
            MemenimScriptCommand command, string name,
            string description, Type type)
        {
            Command = command;

            Name = NormalizeName(name);
            OriginalDescription = NormalizeDescription(description);
            Type = type;
        }

        private static string NormalizeName(string name)
        {
            return name
                .Trim(' ', '_')
                .Replace(' ', '-')
                .Replace('_', '-');
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

        internal string GetBaseLocalizationKey()
        {
            var baseLocalizationKey =
                Command?.GetBaseLocalizationKey();

            return $"{baseLocalizationKey}|[{Name}]_parameter";
        }

        private string GetDescriptionLocalizationKey()
        {
            var baseLocalizationKey =
                GetBaseLocalizationKey();

            return $"{baseLocalizationKey}-description";
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
    }
}
