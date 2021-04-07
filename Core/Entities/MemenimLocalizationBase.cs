using System;

namespace Memenim.Scripting.Core.Entities
{
    public abstract class MemenimLocalizationBase : IMemenimScriptBindable
    {
        public Type BaseType { get; }
        public Type NotImplementedType { get; }
        public MemenimScriptBindTargetType BindTarget { get; }

        public bool IsImplemented
        {
            get
            {
                var type = GetType();

                return type != BaseType
                       && type != NotImplementedType;
            }
        }

        protected MemenimLocalizationBase()
        {
            BaseType = MemenimScriptUtils.GetBaseType(GetType());
            NotImplementedType = MemenimScriptUtils.GetNotImplementedType(BaseType);
            BindTarget = MemenimScriptUtils.GetBindTarget(BaseType);
        }

        public abstract string GetLocalized(string key);
        public abstract TOut GetLocalized<TOut>(string key);

        public abstract string TryGetLocalized(string key);
        public abstract TOut TryGetLocalized<TOut>(string key);
    }
}
