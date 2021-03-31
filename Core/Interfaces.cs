using System;

namespace Memenim.Scripting.Core
{
    public interface IMemenimScriptBindable
    {
        Type BaseType { get; }
        Type NotImplementedType { get; }
        MemenimScriptBindTargetType BindTarget { get; }
    }
}
