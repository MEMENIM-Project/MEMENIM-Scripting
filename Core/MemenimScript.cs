using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Memenim.Scripting.Core.Entities;
using Memenim.Scripting.Core.Entities.NotImplemented;

namespace Memenim.Scripting.Core
{
    public static class MemenimScript
    {
        private static readonly Dictionary<MemenimScriptBindTargetType, MemenimScriptBindReference> BindReferenceMap;

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
                GetNotImplementedType(typeof(T));

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

                var target = GetBindTarget(type);
                var reference = new MemenimScriptBindReference(field);
                var value = reference.Value;

                BindReferenceMap[target] = reference;

                BindImplementation(value, GetBaseType(value));
            }
        }



        internal static Type GetBaseType<T>(
            T value)
            where T : class, IMemenimScriptBindable
        {
            return value.BaseType;
        }
        internal static Type GetBaseType(
            MemenimScriptBindTargetType target)
        {
            switch (target)
            {
                case MemenimScriptBindTargetType.Client:
                    return typeof(MemenimClientBase);
                case MemenimScriptBindTargetType.Terminal:
                    return typeof(MemenimTerminalBase);
                case MemenimScriptBindTargetType.Log:
                    return typeof(MemenimLogBase);
                case MemenimScriptBindTargetType.Dialog:
                    return typeof(MemenimDialogBase);
                case MemenimScriptBindTargetType.Unknown:
                default:
                    return null;
            }
        }
        internal static Type GetBaseType(
            Type type)
        {
            if (typeof(MemenimClientBase).IsAssignableFrom(type))
                return typeof(MemenimClientBase);
            else if (typeof(MemenimTerminalBase).IsAssignableFrom(type))
                return typeof(MemenimTerminalBase);
            else if (typeof(MemenimLogBase).IsAssignableFrom(type))
                return typeof(MemenimLogBase);
            else if (typeof(MemenimDialogBase).IsAssignableFrom(type))
                return typeof(MemenimDialogBase);

            return null;
        }

        internal static Type GetNotImplementedType<T>(
            T value)
            where T : class, IMemenimScriptBindable
        {
            return value.NotImplementedType;
        }
        internal static Type GetNotImplementedType(
            MemenimScriptBindTargetType target)
        {
            switch (target)
            {
                case MemenimScriptBindTargetType.Client:
                    return typeof(MemenimClientNotImplemented);
                case MemenimScriptBindTargetType.Terminal:
                    return typeof(MemenimTerminalNotImplemented);
                case MemenimScriptBindTargetType.Log:
                    return typeof(MemenimLogNotImplemented);
                case MemenimScriptBindTargetType.Dialog:
                    return typeof(MemenimDialogNotImplemented);
                case MemenimScriptBindTargetType.Unknown:
                default:
                    return null;
            }
        }
        internal static Type GetNotImplementedType(
            Type type)
        {
            if (typeof(MemenimClientBase).IsAssignableFrom(type))
                return typeof(MemenimClientNotImplemented);
            else if (typeof(MemenimTerminalBase).IsAssignableFrom(type))
                return typeof(MemenimTerminalNotImplemented);
            else if (typeof(MemenimLogBase).IsAssignableFrom(type))
                return typeof(MemenimLogNotImplemented);
            else if (typeof(MemenimDialogBase).IsAssignableFrom(type))
                return typeof(MemenimDialogNotImplemented);

            return null;
        }

        internal static MemenimScriptBindTargetType GetBindTarget<T>(
            T value)
            where T : class, IMemenimScriptBindable
        {
            return value.BindTarget;
        }
        internal static MemenimScriptBindTargetType GetBindTarget(
            Type type)
        {
            if (typeof(MemenimClientBase).IsAssignableFrom(type))
                return MemenimScriptBindTargetType.Client;
            else if (typeof(MemenimTerminalBase).IsAssignableFrom(type))
                return MemenimScriptBindTargetType.Terminal;
            else if (typeof(MemenimLogBase).IsAssignableFrom(type))
                return MemenimScriptBindTargetType.Log;
            else if (typeof(MemenimDialogBase).IsAssignableFrom(type))
                return MemenimScriptBindTargetType.Dialog;

            return MemenimScriptBindTargetType.Unknown;
        }

        internal static MemenimScriptBindReference GetBindReference<T>(
            T value)
            where T : class, IMemenimScriptBindable
        {
            var target = GetBindTarget(value);

            return GetBindReference(target);
        }
        internal static MemenimScriptBindReference GetBindReference(
            MemenimScriptBindTargetType target)
        {
            if (!BindReferenceMap.ContainsKey(target))
                return null;

            return BindReferenceMap[target];
        }
        internal static MemenimScriptBindReference GetBindReference(
            Type type)
        {
            var target = GetBindTarget(type);

            return GetBindReference(target);
        }


        private static (bool IsSuccess, IMemenimScriptBindable Implementation) TryGetImplementation(
            Type targetType, Type defaultType = null)
        {
            targetType = GetBaseType(targetType);

            if (defaultType == targetType)
                defaultType = null;

            if (!typeof(IMemenimScriptBindable).IsAssignableFrom(targetType))
                return (false, null);

            if (defaultType != null && !targetType.IsAssignableFrom(defaultType))
                return (false, null);

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
            targetType = GetBaseType(targetType);

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

            var reference = GetBindReference(targetType);

            reference.Value = result.Implementation;

            return true;
        }

        private static void BindImplementation<T>(
            T value, Type defaultType)
            where T : class, IMemenimScriptBindable
        {
            var targetType = value?.GetType() ?? typeof(T);

            targetType = GetBaseType(targetType);

            if (defaultType == targetType)
                defaultType = null;

            if (!targetType.IsAssignableFrom(defaultType))
            {
                throw new ArgumentException(
                    $"{nameof(defaultType)} must be derived from the <{typeof(T).Name}> class",
                    nameof(defaultType));
            }

            if (defaultType == null)
            {
                BindImplementationInternal(value);

                return;
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

            targetType = GetBaseType(targetType);

            if (defaultType == targetType)
                defaultType = null;

            if (defaultValue != null
                && defaultValue.GetType() == targetType)
            {
                defaultValue = null;
            }

            if (TryBindImplementation(targetType, defaultType))
                return;

            var reference = GetBindReference(value);

            reference.Value = defaultValue;
        }



        public static void BindImplementation<T>(
            MemenimScriptBindTargetType target, T value)
            where T : class, IMemenimScriptBindable
        {
            if (value == null)
                return;

            Type baseType = GetBaseType(target);

            if (baseType == null)
                return;

            if (!baseType.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException(
                    $"{nameof(value)} must be derived from the {baseType.Name} class",
                    nameof(value));
            }

            var reference = GetBindReference(target);

            reference.Value = value;
        }
    }
}
