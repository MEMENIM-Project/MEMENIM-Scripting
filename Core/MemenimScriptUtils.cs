using System;
using Memenim.Scripting.Core.Entities;
using Memenim.Scripting.Core.Entities.NotImplemented;

namespace Memenim.Scripting.Core
{
    internal static class MemenimScriptUtils
    {
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
                case MemenimScriptBindTargetType.Localization:
                    return typeof(MemenimLocalizationBase);
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
            else if (typeof(MemenimLocalizationBase).IsAssignableFrom(type))
                return typeof(MemenimLocalizationBase);

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
                case MemenimScriptBindTargetType.Localization:
                    return typeof(MemenimLocalizationNotImplemented);
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
            else if (typeof(MemenimLocalizationBase).IsAssignableFrom(type))
                return typeof(MemenimLocalizationNotImplemented);

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
            else if (typeof(MemenimLocalizationBase).IsAssignableFrom(type))
                return MemenimScriptBindTargetType.Localization;

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
            return MemenimScript.GetBindReference(target);
        }
        internal static MemenimScriptBindReference GetBindReference(
            Type type)
        {
            var target = GetBindTarget(type);

            return GetBindReference(target);
        }
    }
}
