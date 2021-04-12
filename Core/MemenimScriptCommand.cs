using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Memenim.Scripting.Core
{
    public sealed class MemenimScriptCommand
    {
        private MemenimScriptBase Script { get; }
        private MethodInfo Method { get; }

        public string Name { get; }
        public string Description { get; }
        public ReadOnlyCollection<MemenimScriptCommandParameter> Parameters { get; }

        public MemenimScriptCommand(
            MemenimScriptBase script, MethodInfo method,
            string name, string description)
        {
            Script = script;
            Method = method;

            Name = NormalizeName(name);
            Description = description;

            Description = GetDescription() ?? string.Empty;

            var parameters = new List<MemenimScriptCommandParameter>();

            foreach (var parameterInfo in Method.GetParameters())
            {
                string parameterName = null;
                string parameterDescription = null;

                if (Attribute.IsDefined(Method, typeof(MemenimScriptCommandParameterAttribute)))
                {
                    var parameterAttribute = (MemenimScriptCommandParameterAttribute)Method
                        .GetCustomAttribute(typeof(MemenimScriptCommandParameterAttribute));

                    parameterName = parameterAttribute?.Name;
                    parameterDescription = parameterAttribute?.Description;
                }

                if (string.IsNullOrWhiteSpace(parameterName))
                    parameterName = parameterInfo.Name;
                if (string.IsNullOrWhiteSpace(parameterDescription))
                    parameterDescription = string.Empty;

                uint parameterNameCounter = 1;

                while (parameters.Find(parameter =>
                           parameter.Name == parameterName)
                       != null)
                {
                    ++parameterNameCounter;

                    parameterName = $"{parameterName} {parameterNameCounter}";
                }

                parameters.Add(new MemenimScriptCommandParameter(this, parameterName,
                    parameterDescription, parameterInfo.ParameterType));
            }

            Parameters = new ReadOnlyCollection<MemenimScriptCommandParameter>(
                parameters);
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
                Script?.GetBaseLocalizationKey();

            return $"{baseLocalizationKey}|{Name}_command";
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
