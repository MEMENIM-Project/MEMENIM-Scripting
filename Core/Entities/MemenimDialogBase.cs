using System;

namespace Memenim.Scripting.Core.Entities
{
    public abstract class MemenimDialogBase : IMemenimScriptBindable
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

        protected MemenimDialogBase()
        {
            BaseType = MemenimScriptUtils.GetBaseType(GetType());
            NotImplementedType = MemenimScriptUtils.GetNotImplementedType(BaseType);
            BindTarget = MemenimScriptUtils.GetBindTarget(BaseType);
        }
    }
}
