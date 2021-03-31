using System;
using Memenim.Scripting.Core.Entities.NotImplemented;

namespace Memenim.Scripting.Core.Entities
{
    public abstract class MemenimLogBase : IMemenimScriptBindable
    {
        public Type BaseType { get; }
        public Type NotImplementedType { get; }
        public MemenimScriptBindTargetType BindTarget { get; }

        protected MemenimLogBase()
        {
            BaseType = GetType();
            NotImplementedType = typeof(MemenimLogNotImplemented);
            BindTarget = MemenimScriptBindTargetType.Log;
        }
    }
}
