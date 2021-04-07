using System;

namespace Memenim.Scripting.Core
{
    public enum MemenimScriptBindTargetType : byte
    {
        Unknown = 0,
        Client = 1,
        Terminal = 2,
        Log = 3,
        Dialog = 4,
        Localization = 5
    }
}
