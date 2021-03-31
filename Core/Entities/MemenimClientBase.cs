using System;
using Memenim.Scripting.Core.Entities.NotImplemented;

namespace Memenim.Scripting.Core.Entities
{
    public abstract class MemenimClientBase : IMemenimScriptBindable
    {
        public Type BaseType { get; }
        public Type NotImplementedType { get; }
        public MemenimScriptBindTargetType BindTarget { get; }

        protected MemenimClientBase()
        {
            BaseType = GetType();
            NotImplementedType = typeof(MemenimClientNotImplemented);
            BindTarget = MemenimScriptBindTargetType.Client;
        }
    }
}
