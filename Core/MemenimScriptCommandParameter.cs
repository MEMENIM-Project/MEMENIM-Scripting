using System;

namespace Memenim.Scripting.Core
{
    public sealed class MemenimScriptCommandParameter
    {
        private readonly MemenimScriptCommand _command;

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
            _command = command;

            Name = NormalizeName(name);
            OriginalDescription = NormalizeDescription(description);
            Type = type;
        }



        internal string GetBaseLocalizationKey()
        {
            var baseLocalizationKey =
                _command?.GetBaseLocalizationKey();

            return $"{baseLocalizationKey}|[{Name}]_parameter";
        }

        private string GetDescriptionLocalizationKey()
        {
            var baseLocalizationKey =
                GetBaseLocalizationKey();

            return $"{baseLocalizationKey}-description";
        }



        private static string NormalizeName(string name)
        {
            return name
                .Trim(' ', '-', '_')
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
    }
}
