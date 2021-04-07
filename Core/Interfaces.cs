using System;

namespace Memenim.Scripting.Core
{
    internal interface IMemenimScriptBindable
    {
        Type BaseType { get; }
        Type NotImplementedType { get; }
        MemenimScriptBindTargetType BindTarget { get; }

        public bool IsImplemented { get; }
    }
}
