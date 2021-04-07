using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Memenim.Scripting.Core.Entities;

namespace Memenim.Scripting.Core
{
    public static class MemenimScript
    {
        private static readonly Dictionary<MemenimScriptBindTargetType, MemenimScriptBindReference> BindReferenceMap;

#pragma warning disable IDE0044 // Make field read only
#pragma warning disable CS0649 // Field is never assigned
        private static MemenimClientBase _client;
        public static MemenimClientBase Client
        {
            get
            {
                return GetImplementation(_client);
            }
        }
        private static MemenimTerminalBase _terminal;
        public static MemenimTerminalBase Terminal
        {
            get
            {
                return GetImplementation(_terminal);
            }
        }
        private static MemenimLogBase _log;
        public static MemenimLogBase Log
        {
            get
            {
                return GetImplementation(_log);
            }
        }
        private static MemenimDialogBase _dialog;
        public static MemenimDialogBase Dialog
        {
            get
            {
                return GetImplementation(_dialog);
            }
        }
        private static MemenimLocalizationBase _localization;
        public static MemenimLocalizationBase Localization
        {
            get
            {
                return GetImplementation(_localization);
            }
        }
#pragma warning restore CS0649 // Field is never assigned
#pragma warning restore IDE0044 // Make field read only

        static MemenimScript()
        {
            BindReferenceMap = new Dictionary<MemenimScriptBindTargetType, MemenimScriptBindReference>();

            BindAll();
        }



        private static T GetImplementation<T>(
            T value)
            where T : class, IMemenimScriptBindable
        {
            if (value != null)
                return value;

            var notImplementedType =
                MemenimScriptUtils.GetNotImplementedType(typeof(T));

            value = (T)Activator.CreateInstance(
                notImplementedType, true);

            return value;
        }

        private static void BindAll()
        {
            foreach (var field in typeof(MemenimScript).GetFields(BindingFlags.Static
                                                                  | BindingFlags.Public
                                                                  | BindingFlags.NonPublic))
            {
                var type = field.FieldType;

                if (!typeof(IMemenimScriptBindable).IsAssignableFrom(type))
                    continue;

                var target = MemenimScriptUtils.GetBindTarget(type);

                if (target == MemenimScriptBindTargetType.Unknown)
                    continue;

                var reference = new MemenimScriptBindReference(field);
                var value = reference.Value;

                BindReferenceMap[target] = reference;

                BindImplementation(value, MemenimScriptUtils.GetBaseType(value));
            }
        }



        internal static MemenimScriptBindReference GetBindReference(
            MemenimScriptBindTargetType target)
        {
            if (target == MemenimScriptBindTargetType.Unknown)
                return null;

            if (!BindReferenceMap.ContainsKey(target))
                return null;

            return BindReferenceMap[target];
        }



        private static (bool IsSuccess, IMemenimScriptBindable Implementation) TryGetImplementation(
            Type targetType, Type defaultType = null)
        {
            targetType = MemenimScriptUtils.GetBaseType(targetType);

            if (defaultType == targetType)
                defaultType = null;

            if (!typeof(IMemenimScriptBindable).IsAssignableFrom(targetType))
                return (false, null);

            if (defaultType != null && !targetType.IsAssignableFrom(defaultType))
                return (false, null);

            var notImplementedType = MemenimScriptUtils.GetNotImplementedType(targetType);
            var assemblies = new List<Assembly>
            {
                Assembly.GetEntryAssembly(),
                Assembly.GetExecutingAssembly(),
                Assembly.GetCallingAssembly()
            };
            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());

            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                    continue;

                var types = assembly.GetTypes()
                    .Where(type => type.IsClass
                                   && (targetType != null && targetType.IsAssignableFrom(type))
                                   && (type != targetType)
                                   && (type != notImplementedType)
                                   && (type != defaultType))
                    .ToArray();

                if (types.Length == 0)
                    continue;

                foreach (var type in types)
                {
                    IMemenimScriptBindable target;

                    try
                    {
                        target = (IMemenimScriptBindable)Activator.CreateInstance(
                            type, true);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (target == null)
                        continue;

                    return (true, target);
                }
            }

            return (false, null);
        }

        private static bool TryBindImplementation(
            Type targetType, Type defaultType = null)
        {
            targetType = MemenimScriptUtils.GetBaseType(targetType);

            if (defaultType == targetType)
                defaultType = null;

            if (!typeof(IMemenimScriptBindable).IsAssignableFrom(targetType))
                return false;

            if (defaultType != null && !targetType.IsAssignableFrom(defaultType))
                return false;

            var result = TryGetImplementation(
                targetType, defaultType);

            if (!result.IsSuccess)
                return false;

            var reference = MemenimScriptUtils.GetBindReference(targetType);

            reference.Value = result.Implementation;

            return true;
        }

        private static void BindImplementation<T>(
            T value, Type defaultType)
            where T : class, IMemenimScriptBindable
        {
            var targetType = value?.GetType() ?? typeof(T);

            targetType = MemenimScriptUtils.GetBaseType(targetType);

            if (defaultType == targetType)
                defaultType = null;

            if (defaultType == null)
            {
                BindImplementationInternal(value);

                return;
            }

            if (!targetType.IsAssignableFrom(defaultType))
            {
                throw new ArgumentException(
                    $"{nameof(defaultType)} must be derived from the <{targetType.Name}> class",
                    nameof(defaultType));
            }

            var defaultValue = (T)Activator.CreateInstance(
                defaultType, true);

            BindImplementationInternal(value, defaultValue);
        }
        private static void BindImplementation<T>(
            T value, T defaultValue = null)
            where T : class, IMemenimScriptBindable
        {
            BindImplementationInternal(value, defaultValue);
        }
        private static void BindImplementationInternal<T>(
            T value, T defaultValue = null)
            where T : class, IMemenimScriptBindable
        {
            var targetType = value?.GetType() ?? typeof(T);
            var defaultType = defaultValue?.GetType();

            targetType = MemenimScriptUtils.GetBaseType(targetType);

            if (defaultType == targetType)
                defaultType = null;

            if (TryBindImplementation(targetType, defaultType))
                return;

            var reference = MemenimScriptUtils.GetBindReference(value);

            reference.Value = defaultValue;
        }



        public static void BindImplementation<T>(
            MemenimScriptBindTargetType target, T value)
            where T : class
        {
            if (target == MemenimScriptBindTargetType.Unknown)
                return;

            if (value == null)
                return;

            if (!(value is IMemenimScriptBindable))
            {
                throw new ArgumentException(
                    $"{nameof(value)} must implement the {nameof(IMemenimScriptBindable)} interface",
                    nameof(value));
            }

            Type baseType = MemenimScriptUtils.GetBaseType(target);

            if (baseType == null)
                return;

            if (!baseType.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException(
                    $"{nameof(value)} must be derived from the target[{target}] base class (in this case - {baseType.Name})",
                    nameof(value));
            }

            var reference = MemenimScriptUtils.GetBindReference(target);

            reference.Value = (IMemenimScriptBindable)value;
        }
    }
}
