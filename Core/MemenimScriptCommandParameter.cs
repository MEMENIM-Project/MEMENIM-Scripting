using System;

namespace Memenim.Scripting.Core
{
    public sealed class MemenimScriptCommandParameter
    {
        private MemenimScriptCommand Command { get; }

        public string Name { get; }
        public string Description { get; }
        public Type Type { get; }

        public MemenimScriptCommandParameter(
            MemenimScriptCommand command, string name,
            string description, Type type)
        {
            Command = command;

            Name = NormalizeName(name);
            Description = description;
            Type = type;

            Description = GetDescription() ?? string.Empty;
        }

        private static string NormalizeName(string name)
        {
            return name
                .Trim(' ')
                .Replace(' ', '-');
        }

        internal string GetBaseLocalizationKey()
        {
            var baseLocalizationKey =
                Command?.GetBaseLocalizationKey();

            return $"{baseLocalizationKey}|{Name}_parameter";
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
                return Description;

            var localizedDescription = MemenimScript.Localization
                .TryGetLocalized(GetDescriptionLocalizationKey());

            if (!string.IsNullOrWhiteSpace(localizedDescription))
                return localizedDescription;

            return Description;
        }
    }
}
