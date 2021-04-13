using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Memenim.Scripting.Core
{
    [RequiredMemenimVersion("0.15.10", null)]
    public abstract class MemenimScriptBase
    {
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

        protected MemenimScriptBase(string name = null,
            string description = null, string company = null,
            IList<string> authors = null)
        {
            OriginalName = NormalizeName(name);
            OriginalDescription = NormalizeDescription(description);
            Company = NormalizeCompany(company);
            Authors = NormalizeAuthors(authors);
            Commands = GetRegisteredCommands();
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

        internal string GetBaseLocalizationKey()
        {
            return "[MemenimScript]_script";
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



        public bool MemenimVersionSatisfied(MemenimVersion version)
        {
            var type = GetType();

            if (!Attribute.IsDefined(type, typeof(RequiredMemenimVersionAttribute)))
                type = typeof(MemenimScriptBase);

            var requiredVersionAttribute = (RequiredMemenimVersionAttribute)type
                .GetCustomAttribute(typeof(RequiredMemenimVersionAttribute));

            if (requiredVersionAttribute == null)
                return true;

            return requiredVersionAttribute.Range
                .IsSatisfied(version);
        }
    }
}
