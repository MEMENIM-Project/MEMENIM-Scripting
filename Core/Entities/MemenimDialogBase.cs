using System;
using Memenim.Scripting.Core.Entities.NotImplemented;

namespace Memenim.Scripting.Core.Entities
{
    public abstract class MemenimDialogBase : IMemenimScriptBindable
    {
        public Type BaseType { get; }
        public Type NotImplementedType { get; }
        public MemenimScriptBindTargetType BindTarget { get; }

        protected MemenimDialogBase()
        {
            BaseType = GetType();
            NotImplementedType = typeof(MemenimDialogNotImplemented);
            BindTarget = MemenimScriptBindTargetType.Dialog;
        }
    }
}
