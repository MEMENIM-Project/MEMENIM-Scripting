using System;
using Memenim.Scripting.Core.Entities.NotImplemented;

namespace Memenim.Scripting.Core.Entities
{
    public abstract class MemenimTerminalBase : IMemenimScriptBindable
    {
        public Type BaseType { get; }
        public Type NotImplementedType { get; }
        public MemenimScriptBindTargetType BindTarget { get; }

        protected MemenimTerminalBase()
        {
            BaseType = GetType();
            NotImplementedType = typeof(MemenimTerminalNotImplemented);
            BindTarget = MemenimScriptBindTargetType.Terminal;
        }

        public abstract void WriteLine(string text);

        public abstract void Clear();
    }
}
