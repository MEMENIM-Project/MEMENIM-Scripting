using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Memenim.Scripting.Core
{
    public sealed class MemenimScriptCommand
    {
        private MemenimScriptBase Script { get; }
        private MethodInfo Method { get; }

        public string Name { get; }
        public string OriginalDescription { get; }
        public string Description
        {
            get
            {
                return GetDescription() ?? string.Empty;
            }
        }
        public ReadOnlyCollection<MemenimScriptCommandParameter> Parameters { get; }

        internal MemenimScriptCommand(
            MemenimScriptBase script, MethodInfo method,
            string name, string description)
        {
            Script = script;
            Method = method;

            Name = NormalizeName(name);
            OriginalDescription = NormalizeDescription(description);

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
                MemenimScriptBase.GetBaseLocalizationKey();

            return $"{baseLocalizationKey}|[{Name}]_command";
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



        public void Execute(IList<object> parameters)
        {
            parameters ??= Array.Empty<object>();

            if (parameters.Count != Parameters.Count)
            {
                throw new ArgumentException(
                    $"Passed parameters count[{parameters.Count}] " +
                    $"does not match expected parameters count[{Parameters.Count}] " +
                    $"for command[{Name}] in script[{Script.Name}]",
                    nameof(parameters));
            }

            for (int i = 0; i < parameters.Count; ++i)
            {
                if (parameters[i].GetType() == Parameters[i].Type)
                    continue;

                object parameter;

                try
                {
                    parameter = Convert.ChangeType(parameters[i], Parameters[i].Type);
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException(
                        $"Passed parameter[Index={i}, Type={parameters[i].GetType().FullName ?? parameters[i].GetType().Name}] " +
                        $"cannot be converted to type[{Parameters[i].Type.FullName ?? Parameters[i].Type.Name}] of expected parameter[{Parameters[i].Name}] " +
                        $"for command[{Name}] in script[{Script.Name}]",
                        ex);
                }

                parameters[i] = parameter;
            }

            var source = Method.IsStatic
                ? null
                : Script;

            var flags = BindingFlags.Public | BindingFlags.NonPublic;
            flags |= Method.IsStatic
                ? BindingFlags.Static
                : BindingFlags.Instance;

            Method.Invoke(source, flags, null,
                parameters.ToArray(), CultureInfo.InvariantCulture);
        }
    }
}
