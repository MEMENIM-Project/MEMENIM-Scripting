using System;
using System.Globalization;
using System.Reflection;

namespace Memenim.Scripting.Core
{
    internal sealed class MemenimScriptBindReference
    {
        private readonly FieldInfo _field;

        public Type Type { get; }
        public MemenimScriptBindTargetType Target { get; }
        public IMemenimScriptBindable Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        internal MemenimScriptBindReference(FieldInfo field)
        {
            _field = field;

            Type = field.FieldType;
            Target = MemenimScriptUtils.GetBindTarget(
                field.FieldType);
        }

        private IMemenimScriptBindable GetValue()
        {
            object value = _field
                .GetValue(null);

            if (value != null)
                return (IMemenimScriptBindable)value;

            var notImplementedType = MemenimScriptUtils
                .GetNotImplementedType(Target);

            value = (IMemenimScriptBindable)Activator.CreateInstance(
                notImplementedType, true);

            return (IMemenimScriptBindable)value;
        }

        private void SetValue(IMemenimScriptBindable value)
        {
            const BindingFlags bindingFlags = BindingFlags.Static
                                              | BindingFlags.Public
                                              | BindingFlags.NonPublic;

            _field.SetValue(null, value, bindingFlags,
                null, CultureInfo.InvariantCulture);
        }
    }
}
